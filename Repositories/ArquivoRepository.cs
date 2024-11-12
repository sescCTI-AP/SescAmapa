using Microsoft.EntityFrameworkCore;
using SiteSesc.Data;
using SiteSesc.Models;
using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SkiaSharp;



namespace SiteSesc.Repositories
{
    public class ArquivoRepository
    {
        private readonly SiteSescContext _dbContext;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly HostConfiguration hostConfiguration;
        private IWebHostEnvironment _env;
        private bool devMode;

        public ArquivoRepository(SiteSescContext dbContext, IWebHostEnvironment env, IConfiguration configuration, UsuarioRepository usuarioRepository)
        {
            hostConfiguration = configuration.GetSection("HostConfig").Get<HostConfiguration>();
            _usuarioRepository = usuarioRepository;
            _env = env;
            _dbContext = dbContext;
            devMode = configuration.GetSection("Development")["mode"] == "development";
        }

        public async Task<Arquivo> Get(int? id)
        {
            if (id != null)
            {
                var arquivo = await _dbContext.Arquivo.FirstOrDefaultAsync(a => a.Id == id);
                return arquivo;
            }
            return null;
        }

        public async Task<Arquivo> Salvar(Arquivo arquivo)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (arquivo != null)
                    {
                        try
                        {
                            await _dbContext.Arquivo.AddAsync(arquivo);
                            await _dbContext.SaveChangesAsync();
                            await transaction.CommitAsync();
                            return arquivo;
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            Console.WriteLine(ex.Message);
                            return null;
                        }
                    }
                    return null;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        }

        public async Task<bool> EditArquvos(List<IFormFile> listaNovosArquivos, SolicitacaoCadastroCliente solicitacao)
        {
            var usuario = await _usuarioRepository.GetUsuarioById(solicitacao.IdUsuario);
            var arquivosAntigos = new List<ArquivoCadastroCliente>();
            if (usuario != null)
            {
                if (listaNovosArquivos.Any())
                {
                    foreach (var novoArquivo in listaNovosArquivos)
                    {
                        var arquivoAntigo = await _dbContext.ArquivoCadastroCliente
                            .FirstOrDefaultAsync(a => a.IdSolicitacaoCadastroCliente == solicitacao.Id && a.Tipo == novoArquivo.Name);
                        if (arquivoAntigo != null)
                        {
                            arquivosAntigos.Add(arquivoAntigo);
                        }
                    }


                    try
                    {
                        var uploadArquivos = await SaveClientFile(listaNovosArquivos, solicitacao, usuario.Cpf);

                        //Remove antigos
                        if (arquivosAntigos != null)
                        {
                            _dbContext.Set<ArquivoCadastroCliente>().RemoveRange(arquivosAntigos);                            
                        }
                        await _dbContext.SaveChangesAsync();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return false;
                    }

                }
            }
            return false;

        }

        public async Task<Arquivo> ProcessImage(string canvas, string entidade)
        {
            try
            {
                var base64 = canvas;
                var byteArray = Convert.FromBase64String(base64.Split(',')[1]);

                using (var inputStream = new MemoryStream(byteArray))
                {
                    // Decodificar a imagem original usando SkiaSharp
                    using (var bitmap = SKBitmap.Decode(inputStream))
                    {
                        using (var outputStream = new MemoryStream())
                        {
                            // Converter a imagem para o formato WebP
                            bitmap.Encode(outputStream, SKEncodedImageFormat.Webp, 90); // 100 é a qualidade

                            outputStream.Seek(0, SeekOrigin.Begin); // Volta o stream para o início

                            // Criar um IFormFile para o arquivo WebP
                            IFormFile webpFile = new FormFile(outputStream, 0, outputStream.Length, "name", "filename.webp")
                            {
                                Headers = new HeaderDictionary()
                            };

                            ContentDisposition cd = new ContentDisposition
                            {
                                FileName = webpFile.FileName
                            };

                            // Chamar UploadArquivo com o arquivo WebP
                            var upload = await UploadArquivo(webpFile, entidade, null);
                            return upload;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Arquivo> UploadArquivo(IFormFile imagem, string entidade, string? identificador, string subDirectory = null, string nome = null)
        {
            var contentRoot = _env.ContentRootPath;
            var extensaoArquivo = Path.GetExtension(imagem.FileName);
            var nomeArquivo = $@"{entidade}_{DateTime.Now.Ticks}{extensaoArquivo}";
            var nomeExibicao = nome;
            if (string.IsNullOrEmpty(nome))
            {
                nomeExibicao = nomeArquivo;
            }
            string diretorio;
            string caminhoAbsoluto;
            string caminhoRelativo;
            string subDirectorio = !string.IsNullOrEmpty(subDirectory) ? $"\\{subDirectory}" : "";
            string subDirecAbsoluto = !string.IsNullOrEmpty(subDirectory) ? $@"\{subDirectory}" : "";
            string subDirecRelativo = !string.IsNullOrEmpty(subDirectory) ? $"/{subDirectory}" : "";

            if (!string.IsNullOrEmpty(identificador))
            {
                diretorio = $"{contentRoot}\\..{hostConfiguration.PastaArquivosSite}{entidade}\\{identificador}{subDirectorio}";
                caminhoAbsoluto = $@"{entidade}\{identificador}{subDirecAbsoluto}";
                caminhoRelativo = $"{entidade}/{identificador}{subDirecRelativo}";
            }
            else
            {
                diretorio = $"{contentRoot}\\..{hostConfiguration.PastaArquivosSite}{entidade}";
                caminhoAbsoluto = entidade;
                caminhoRelativo = entidade;
            }

            if (!Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }

            var destinoFisico = Path.Combine(diretorio, nomeArquivo);

            using (var fileStream = new FileStream(destinoFisico, FileMode.Create))
            {
                await imagem.CopyToAsync(fileStream);
            }

            if (System.IO.File.Exists(destinoFisico))
            {
                var arquivoSubmit = new Arquivo
                {
                    NomeArquivo = nomeExibicao,
                    CaminhoAbsoluto = $@"{caminhoAbsoluto}\{nomeArquivo}",
                    CaminhoVirtual = $"{caminhoRelativo}/{nomeArquivo}",
                    Extensao = extensaoArquivo.ToLower()
                };
                var arquivo = await Salvar(arquivoSubmit);
                return arquivo;
            }
            return null;
        }

        public List<IFormFile> GetIFormFileFields(object obj)
        {
            var type = obj.GetType();
            var fields = type.GetProperties();
            var fileList = new List<IFormFile>();

            foreach (var field in fields)
            {
                if (field.PropertyType == typeof(IFormFile))
                {
                    var value = field.GetValue(obj) as FormFile;
                    fileList.Add(value);
                }
            }

            return fileList;
        }

        public async Task<bool> SaveClientFile(List<IFormFile> arquivos, SolicitacaoCadastroCliente solicitacao, string identificador)
        {
            try
            {
                if (arquivos.Any())
                {
                    foreach (var arquivo in arquivos)
                    {
                        if (arquivo != null)
                        {
                            var file = await UploadArquivo(arquivo, "documento-cliente", identificador);
                            var arquivoCliente = new ArquivoCadastroCliente
                            {
                                IdArquivo = file.Id,
                                IdSolicitacaoCadastroCliente = (Guid)solicitacao.Id,
                                Tipo = arquivo.Name
                            };

                            await _dbContext.ArquivoCadastroCliente.AddAsync(arquivoCliente);
                        }
                    }
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}