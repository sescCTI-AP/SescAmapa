const menu = document.querySelectorAll(".itemMenu")

$(window).on("load", function(){
  console.log('a')
    if( sessionStorage.getItem("exibirAnimacaologo") ){
      //se existir o registro que ja animou, apenas para a animaçao, se nao da o tempo da animação e registra que ja animou
      $(".logoImg1").animate({opacity: '100%'}, 1000);
      $(".logoImg2").animate({opacity: '100%'}, 1000);
    }else{
      $(".logoImg1").animate({opacity: '100%'}, 1000);
      $(".logoImg2").animate({opacity: '100%'}, 1000);
      $(".logoImg1").addClass("animaImg1")
      $(".logoImg2").addClass("animaI2")
      $(".logoImg2 p").addClass("animaI2P")
      $(".logoImg2 img").addClass("animaI2Img")
      
      setTimeout(function(){ 
        sessionStorage.setItem("exibirAnimacaologo", "true");
      }, 5000);
      
    }
})   


menu.forEach(function(elemento) {
  const elementoAtual = elemento;
  elementoAtual.addEventListener("click", function (event) {
    event.preventDefault()
    classeAbaFilme = (event.target.classList[1]).toString();
    conteudoAbaFilme = (event.target.classList[1]).toString();

    const listaFiltradaAba = document.querySelectorAll("."+classeAbaFilme)
    const listaFiltradaCont = document.querySelectorAll(".Sub"+classeAbaFilme)
    removeActiveClass(listaFiltradaAba, listaFiltradaCont)
    elementoAtual.classList.add("menuAtivo")

    if(event.target.innerText==="Sinopse"){
      ativarelemento(listaFiltradaCont, "sinopse")
    }else
    if(event.target.innerText==="Ficha Técnica"){
      ativarelemento(listaFiltradaCont, "fichaTecnica")
    }else
    if(event.target.innerText==="Trailer"){
      ativarelemento(listaFiltradaCont, "trailer")
    }else
    if(event.target.innerText==="Ingresso"){
      ativarelemento(listaFiltradaCont, "ingresso")
    }
  
  
  })
})


const removeActiveClass = (aba, cont) => {
  aba.forEach(function(element) {
    element.classList.remove("menuAtivo");
  })
  cont.forEach(function(element) {
    element.classList.remove("itemAtivo");
  })
}


const ativarelemento = (array, filtro) =>{
  array.forEach(function(el) {
    if(el.classList.contains(filtro) == true){
      el.classList.add("itemAtivo")
    }
  })
}


function selecionaCadeira(e){
  const cadeiras          = document.querySelectorAll(".cadeiraAtivaContent")
  const botaoProsseguir   = document.getElementById("BotaoProsseguir")
  const mostrarAssntEscld = document.getElementById("AssentoEscolhido")

  removeCadeiraAtiva(cadeiras)
  e.classList.add("cadeiraSelecionada")
  mostrarAssntEscld.innerText = e.innerText

  botaoProsseguir.classList.add("button-Prosseguir")
  botaoProsseguir.classList.remove("button-Inativo");
  botaoProsseguir.disabled = false;
  botaoProsseguir.setAttribute("aria-describedby", "Avançar para próxima etapa");
  sessionStorage.setItem("AssentoSelecionado", e.innerText);
  
}


const removeCadeiraAtiva = (aba) => {
  aba.forEach(function(element) {
    element.classList.remove("cadeiraSelecionada");
  })
}
