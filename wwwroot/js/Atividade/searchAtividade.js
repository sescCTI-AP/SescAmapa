var cduop = "";
document.addEventListener('DOMContentLoaded', function () {
    const container = document.getElementById('atividadesContainer');
    const loadingImage = document.createElement('img');
    loadingImage.src = '/images/loading.gif';
    loadingImage.style.width = '70px';
    container.appendChild(loadingImage);
    fetch('/get-atividades')
        .then(response => response.json())
        .then(data => {
            container.removeChild(loadingImage);
            console.log(data);
            data.forEach(atividade => {

                let nomeUnidade = '';
                if(atividade.courseLocation=="CENTRO DE ATIVIDADES DO SESC BELÉM - SESC DOCA  "){
                    nomeUnidade = "Sesc Doca";
                }else
                if(atividade.courseLocation=="CENTRO DE ATIVIDADES SESC CASTANHAL"){
                    nomeUnidade = "Sesc em Castanhal";
                }else
                if(atividade.courseLocation=="CENTRO DE ATIVIDADES DO SESC SANTAREM"){
                    nomeUnidade = "Sesc em Santarém";
                }else
                if(atividade.courseLocation=="CENTRO DE ATIVIDADES SESC ANANINDEUA"){
                    nomeUnidade = "Sesc em Ananindeua";
                }else
                if(atividade.courseLocation=="CASA DE MÚSICA DO SESC"){
                    nomeUnidade = "Sesc Casa da Música";
                }else
                if(atividade.courseLocation=="SESC ALTAMIRA"){
                    nomeUnidade = "Sesc em Altamira";
                }else
                if(atividade.courseLocation=="CENTRO DE ATIVIDADES - SESC MARABÁ"){
                    nomeUnidade = "Sesc em Marabá";
                }else
                if(atividade.courseLocation=="CENTRO CULTURAL DE BELÉM - SESC BOULEVARD"){
                    nomeUnidade = "Sesc Ver-o-Peso";
                }else{
                    nomeUnidade = atividade.courseLocation
                }
                
                const htmlContent = `
                           <div class="col-xxl-3 col-lg-4 col-md-6 card-pill" data-category="category-${atividade.idCategory}">
                            <div class="course-item mb-30">
                                <div class="course-img">
                                    <img src="${atividade.courseImage}" alt="${atividade.courseTitle}" loading="lazy" >
                                </div>
                                <div class="course-content">
                                        <div class="course-content-top">
                                            <div class="course-top-title">
                                                <h6>${atividade.courseCategory}</h6>
                                            </div>
                                        </div>
                                        <h5 class="course-content-title"><a href="/atividade/detalhes/${atividade.cdElement}">${atividade.courseTitle}</a></h5>
                                        <div class="course-content-bottom">
                                            <div class="course-bottom-info">
                                                <strong class="atividade-unidade-nome">${nomeUnidade}</strong>
                                            </div>
                                            <div class="course-bottom-price">
                                                <label>A partir de</label> <span> R$ ${atividade.coursePrice}</span>
                                            </div>
                                        </div>
                                        <a href="/atividade/detalhes/${atividade.cdElement}" class="theme-btn course-btn">Clique aqui</a>
                                    <div class="course-hover-cart-btn">
                                    </div>
                                </div>
                            </div>
                        </div>
                    `;
                container.insertAdjacentHTML('beforeend', htmlContent);
            });
        })
        .catch(error => {
            container.removeChild(loadingImage);
            console.error('Erro ao buscar matrículas:', error);
        });
});


function atividadesPorUop() {
    var uop = $('#select-atv-uop').val();
    cduop = uop;

    const container = document.getElementById('atividadesContainer');
    container.innerHTML = '';
    const loadingImage = document.createElement('img');
    loadingImage.src = '/images/loading.gif';
    loadingImage.style.width = '70px';
    container.appendChild(loadingImage);
    fetch('/get-atividades-uop/' + uop)
        .then(response => response.json())
        .then(data => {
            container.removeChild(loadingImage);
            console.log(data);
            data.forEach(atividade => {
                const htmlContent = `
                                   <div class="col-xxl-3 col-lg-4 col-md-6 card-pill" data-category="category-${atividade.idCategory}">
                                    <div class="course-item mb-30">
                                        <div class="course-img">
                                            <img src="${atividade.courseImage}" alt="${atividade.courseTitle}" loading="lazy" >
                                        </div>
                                        <div class="course-content">
                                                <div class="course-content-top">
                                                    <div class="course-top-title">
                                                        <h6>${atividade.courseCategory}</h6>
                                                    </div>
                                                </div>
                                                <h5 class="course-content-title"><a href="/atividade/detalhes/${atividade.cdElement}">${atividade.courseTitle}</a></h5>
                                                <div class="course-content-bottom">
                                                    <div class="course-bottom-info">
                                                        <span>${atividade.courseLocation}</span>
                                                    </div>
                                                    <div class="course-bottom-price">
                                                        <label>A partir de</label><span>${atividade.coursePrice}</span>
                                                    </div>
                                                </div>
                                                        <a href="/atividade/detalhes/${atividade.cdElement}" class="theme-btn course-btn">Clique aqui</a>
                                            <div class="course-hover-cart-btn">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            `;
                container.insertAdjacentHTML('beforeend', htmlContent);
            });
        })
        .catch(error => {
            container.removeChild(loadingImage);
            console.error('Erro ao buscar matrículas:', error);
        });
}


function getAtividadesId(id) {
    const container = document.getElementById('atividadesContainer');
    container.innerHTML = '';
    const loadingImage = document.createElement('img');
    loadingImage.src = '/images/loading.gif';
    loadingImage.style.width = '70px';
    container.appendChild(loadingImage);

    fetch('/get-atividades/' + id + '/' + cduop)
        .then(response => response.json())
        .then(data => {
            container.removeChild(loadingImage);
            console.log(data);
            data.forEach(atividade => {
                const htmlContent = `
                               <div class="col-xxl-3 col-lg-4 col-md-6 card-pill" data-category="category-${atividade.idCategory}">
                                <div class="course-item mb-30">
                                    <div class="course-img">
                                        <img src="${atividade.courseImage}" alt="${atividade.courseTitle}" loading="lazy" >
                                    </div>
                                    <div class="course-content">
                                            <div class="course-content-top">
                                                <div class="course-top-title">
                                                    <h6>${atividade.courseCategory}</h6>
                                                </div>
                                            </div>
                                            <h5 class="course-content-title"><a href="/atividade/detalhes/${atividade.cdElement}">${atividade.courseTitle}</a></h5>
                                            <div class="course-content-bottom">
                                                <div class="course-bottom-info">
                                                    <span>${atividade.courseLocation}</span>
                                                </div>
                                                <div class="course-bottom-price">
                                                    <label>A partir de</label><span>${atividade.coursePrice}</span>
                                                </div>
                                            </div>
                                                    <a href="/atividade/detalhes/${atividade.cdElement}" class="theme-btn course-btn">Clique aqui</a>
                                        <div class="course-hover-cart-btn">
                                        </div>
                                    </div>
                                </div>
                            </div>
                        `;
                container.insertAdjacentHTML('beforeend', htmlContent);
            });
        })
        .catch(error => {
            container.removeChild(loadingImage);
            console.error('Erro ao buscar matrículas:', error);
        });
}


document.querySelectorAll('.pill').forEach(pill => {
    pill.addEventListener('click', function () {

        const category = this.getAttribute('data-category');

        document.querySelectorAll('#atividadesContainer .card-pill').forEach(card => {
            if (card.getAttribute('data-category') === category) {
                card.classList.remove('inative');
                card.classList.add('active');
                console.log("sim");
            } else {
                card.classList.remove('active');
                card.classList.add('inative');
                console.log("não");
            }
        });
    });
});