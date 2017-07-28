define(['jquery', 'jqueryui'], function ($) {
    $(function ($) {
        $.fn.SelectTag = function (options) {
            var tokenfieval;
            var $this = $(this);
            $.ajax({
                type: "get",
                url: options.dataUrl,
                data: { topNumber: options.selectionNum, tenantTypeId: options.tenantTypeId, t: new Date().getTime() },
                async: false,
                success: function (data) {
                    if (!data && data == 'undefined') {
                        return false;
                    }
                    tokenfie(data);
                }
            });

            function tokenfie(data) {
                //标签
                $this.tokenfield({
                    limit: options.maxcount,
                    autocomplete: {
                        source: data,
                        delay: 100
                    },
                    showAutocompleteOnFocus: options.showAuto,
                    delimiter: [',', ' ', '-', '_']
                });
            }

            $this.on('tokenfield:createtoken', function (event) {
                var existingTokens = $(this).tokenfield('getTokens');
                if (event.attrs.value.length > options.valuelength) {
                    layer.msg("标签长度不能超过" + options.valuelength, {
                        icon: 2
                    });
                    return false;
                }
                $.each(existingTokens, function (index, token) {
                    if (token.value === event.attrs.value) event.preventDefault();
                });
            });
        }
    });

    $(function ($) {
        $('input[data-plugin="SelectTag"]').livequery(function () {
            var $this = $(this);
            $(this).SelectTag({
                name: $this.data('name'),
                selectionNum: $this.data('selectionnum'),
                value: $this.data('value'),
                maxcount: $this.data('limit'),
                dataUrl: $this.data('sourceurl') == '' ? '/Common/JsonTags' : $this.data('sourceurl'),
                parentId: $this.data('parentid'),
                showAuto: $this.data('showautocompleteonfocus') == 'True' ? true : false,
                valuelength: $this.data('valuelength'),
                validation: $this.data('validation'),
                tenantTypeId: $this.data('tenantTypeId')
            });
        });
    });
})
