
$(function () {

    $('.modal').on('show.bs.modal', function () {       
        $('#body-content').addClass('blur').after($(this));
    })

    $('.modal').on('hidden.bs.modal', function (e) {
        $('#body-content').removeClass('blur')
    })

    //----------CHAMADA AJAX DO MODAL PART VIEW----------
    $(document).on("click", ".call-modal", function () {
        var url = $(this).data('id');
        console.log(url)
        $("#modalGeral").load(url, function () {
            $("#modalGeral").modal("show").css('z-index', '9999');
        })
    });

    //<---ROTACIONAR ICONE V--->
    $(function () {
        $('.btn-collapse').click(function () {
            $('.arrow-collapse').not($(this).children('.arrow-collapse')).removeClass('rotate-180')
            $(this).children('.arrow-collapse').toggleClass('rotate-180')
        })
    })

    $("#sidebar").perfectScrollbar()

    var fullHeight = function () {

        $('.js-fullheight').css('height', $(window).height());
        $(window).resize(function () {
            $('.js-fullheight').css('height', $(window).height());
        });

    };
    fullHeight();
    //< ----DINÂMICA DO SIDEMENU ---->
    $('.side-submenu.active').parent('li').parent('ul').addClass('show')
    $('#Open-Close-Menu').click(function () {

        if ($(window).width() <= 992) {
            $('#responsiveCloseButton').click(function () { $('#Open-Close-Menu').click(); focusOutElement(true); })
            focusInElement(true, $('#div-sidebar'), $('#sidebar'))
            dinamicaSideMenu(this)
            $('#background-to-focus-element').attr("onclick", `dinamicaSideMenuResponsivo('#Open-Close-Menu')`)
        }
        else {
            dinamicaSideMenu(this)
        }
    });



});

function dinamicaSideMenuResponsivo(clickedElement) {
    dinamicaSideMenu(clickedElement)
    focusOutElement(true)
}



function dinamicaSideMenu(clickedElement) {

    $(clickedElement).toggleClass('active');
    $('#div-sidebar').toggleClass('active');
    $('#content').toggleClass('active');
    $('#sidebar').toggleClass('active active-on-hover');
    $('.label-sidebar-menu').toggleClass('active');
    $('.li-sidebar').toggleClass('hover-enable');
    $('.btn-menu-cliente').toggleClass('hover-enable active').promise().done(function () {
        if (!$(this).hasClass('hover-enable')) {
            $(this).each(function () {
                $(this).attr('data-title-aux', $(this).attr('data-original-title'))
                $(this).removeAttr('data-original-title')
            })
        } else {
            $(this).each(function () {
                $(this).attr('data-original-title', $(this).attr('data-title-aux'));
            })
        }
    })
    $('#menu-top').toggleClass('active');


}

//$('#sidebar').hover(function () {
//    if ($(this).hasClass('active-on-hover')) {
//        $(this).toggleClass('active')
//}

//})

