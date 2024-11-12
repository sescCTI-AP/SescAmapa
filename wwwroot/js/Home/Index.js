$(document).ready(function () {
    
    $('.owl-carousel.carousel-vendas').addClass($('#for-two-rows').val())
    if ($(window).width() >= 992) {

        $.each($(".two-rows-carousel"), function (key, val) {
            //console.log('rows')
            var item = $(this).find('.item');
            $.each($(item), function (key, val) {
                if (key % 2 == 0) {
                    $(this).add($(this).next('.item')).wrapAll('<div class="item__col" />');
                }
            });
        });
    }
    //console.log($('.carousel-noticias >*.owl-stage'))
    $('.carousel-noticias>.owl-stage-outer>.owl-stage').addClass('py-lg-4')

    //$('#btn-aside-home').click(function () {
    //    $(this).toggleClass('show')
    //    $('#aside-menu-fixed').toggleClass('show')
    //})


    $('.carousel-indicators').prepend($('.indicadores'))

    $('[data-toggle="tooltip"]').tooltip();

    //---RODAPÉ---
    $('footer').removeClass("bg-dark").css("background-color", "rgb(168,207,69)");

    //---VÍDEOS---
    $(".video-img-container").each(function () {
        var div = $(this).find(".title");
        var video = $(this).data('video');
        (function () {
            var youtubeAPI = "https://www.googleapis.com/youtube/v3/videos?id=" + video + "&key=AIzaSyCAZAtauaehDGN3076aaGqYBQ15CmmH4qA&fields=items(id,snippet(title))&part=snippet";
            $.getJSON(youtubeAPI, {
            })
                .done(function (data) {
                    div.html(data.items[0].snippet.title);
                });
        })();
    });

    //---CAROUSEL VENDAS---
    $('.owl-carousel.carousel-vendas').owlCarousel({
        autoplay:true,
        margin: 10,
        nav: true,
        center: false,
        loop: true,
        responsive: {
            0: {
                items: 1
            },
            768: {
                items: 2
            },
            960: {
                items: 3
            },
            1200: {
                items: 4
            }
        }
    })

    //---CAROUSEL NOTICIAS---
    $('.owl-carousel.carousel-noticias').owlCarousel({
        loop: true,
        margin: 10,
        nav: true,
        center: false,
        loop: false,
        responsive: {
            0: {
                items: 1
            },
            768: {
                items: 2
            },
            960: {
                items: 3
            },
            1200: {
                items: 4
            }
        }

    })

    //---CAROUSEL CANAL---
    $('.owl-carousel.carousel-canal').owlCarousel({
        loop: true,
        margin: 10,
        nav: true,
        center: false,
        loop: false,
        responsive: {
            0: {
                items: 1
            },
            768: {
                items: 2
            },
            960: {
                items: 3
            }
        }

    });


})
