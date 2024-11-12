var docuManager = {
    searchPlugin: function (plugin) {
        var $list = $('div.list-group');
        if (plugin === 'All') {
            $list.children().show();
        } else {
            $list.children().hide();
            $list.find('a[data-plugin="' + plugin + '"]').show();
        }
    },

    searchItem: function (query) {
        var $list = $('div.list-group');
        if (!query) {
            $list.children().show();
        } else {
            $list.children().hide();
            $list.find('a h4:contains("' + query + '")').parent().show();
        }
    },

    updateDocLinks: function (newHash) {
        var $docLinks = $('ul li a[data-role="docLink"]');
        $docLinks.each(function () {
            var href = $(this).attr('href'),
                hashPos = href.indexOf('#');
            if (hashPos > -1) {
                $(this).attr('href', href.substr(0, hashPos) + newHash);
            } else {
                $(this).attr('href', href + newHash);
            }
        });
    },

    openCode: function (e) {
        var $container = $(this).closest('div[data-role="example"]');
        $container.find('iframe.example-frame').hide();
        $container.find('pre.example-code').show();
    },

    openPreview: function (e) {
        var $container = $(this).closest('div[data-role="example"]'),
            $iframe = $container.find('iframe.example-frame');
        $container.find('pre.example-code').hide();
        $iframe.show();
        $iframe.attr('src', $iframe.data('url'));
    },

    openEdit: function (e) {
        window.open($(this).data('href'));
    }
};

$(document).ready(function () {
    var pluginName;
    if (window.location.hash) {
        pluginName = $('ul[aria-labelledby="plugingName"] a[href="' + window.location.hash + '"]').text();
        $('button#plugingName span[role="label"]').html(pluginName + ' &nbsp;');
        docuManager.searchPlugin(pluginName);
        docuManager.updateDocLinks(window.location.hash);
    }
    $('a[role="menuitem"]').on('click', function (e) {
        var $item = $(this),
            value = $item.text(),
            $dropdown = $item.closest('div.dropdown'),
            $label = $dropdown.find('button span[role="label"]');
        $label.text(value);
        docuManager.searchPlugin(value);
        docuManager.updateDocLinks($item.attr('href'));
    });
    $('#btnSearch').on('click', function (e) {
        docuManager.searchItem($('#txtQuery').val());

    });
    $('button[data-role="open-code"]').on('click', docuManager.openCode);
    $('button[data-role="open-preview"]').on('click', docuManager.openPreview);
    $('button[data-role="open-edit"]').on('click', docuManager.openEdit);

});