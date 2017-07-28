define(['jquery', 'webuploader'], function ($, WebUploader) {
    $(function ($) {
        $('[data-plugin="fileuploader"]').livequery(function () {
            var $this = $(this),
                id = $this.attr('id'),
                targetBtn = $this.data('selector'),
                showProgress = $this.attr("data-show-progress") == "false" ? false : true,
                $progress = $('<div class="progress" style="display:none;margin: 12px 0px 0px 0px">' +
                '<div class="progress-bar" role="progressbar" aria-valuenow="60" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div></div><span></span>');
            var $from = $this.parents('form');
            if (showProgress) {
                if ($from && $from.length > 0) {
                    $this.before($progress);
                    $progress.css({ width: '170px', height: '10px' });
                }
                else {
                    $this.parent().before($progress);
                    $progress.css({ width: '600px', margin: '100px auto' });
                }
            }
            //解决模态框定位问题
            setTimeout(function () {
                var maxcount = $this.data('maxcount');
                var multiple = ($this.data('multiple') == "false" || $this.data('multiple') == false) ? false : true;
                maxcount = maxcount == 0 ? 20 : maxcount;

                var uploader = WebUploader.create({
                    auto: true,
                    pick: {
                        id: targetBtn ? targetBtn : ('#' + id)
                    },
                    fileNumLimit: maxcount,
                    server: $this.data('uploadurl'),

                    swf: '/js/lib/webuploader/css/Uploader.swf',
                    fileVal: $this.attr('name'),
                    accept: { title: '请选择文件', extensions: $this.data('extensions'), multiple: multiple },
                    formData: { 'ownerId': $this.data('ownerid'), 'tenantTypeId': $this.data('tenanttypeid'), 'associateId': $this.data('associateid') }
                });
                //进度条
                if (showProgress) {
					uploader.on('startUpload', function () {
                        $progress.show();
                        $this.addClass('webuploader-element-invisible');
                    });
                	uploader.on('uploadProgress', function (file, percentage) {
	            	    if (percentage <= 0.8) {
                                $progress.children('div').css('width', Math.round(percentage * 100) + '%');
                                $progress.eq(1).text(Math.round(percentage * 100) + '%');
                                $("#photoslist").append("<div class='judge'></div>");

                        }
                        else if (percentage == 1) {
                                $progress.children('div').css('width', '65%');
                                $progress.eq(1).text('65%');
                                $("#photoslist").append("<div class='judge'></div>");
                        }
		            });
                	uploader.on("uploadSuccess", function (file, response) {
                     var progressNum=uploader.getStats().progressNum;
                        if(progressNum==0){
	                        uploader.reset();
	                        $progress.children('div').css('width', '100%');
	                        setTimeout(function () {
	                            $progress.fadeOut('slow', function () {
	                                $(this).children('div').css('width', '0%');
	                                $this.removeClass('webuploader-element-invisible');
	                            });
	                        }, 1200);
	                        //方法回调
	                        if ($this.data("uploadsuccess")) {
	                            var funName = $this.data("uploadsuccess"),
	                                // success = eval(funName);
	                               success = eval("$.fn." + funName)
		                            success.call(this, file, response, uploader);
		                    }
                        }
                    });
				} else {
                    uploader.on("uploadSuccess", function (file, response) {

                        uploader.reset();
                        setTimeout(function () {
                            $progress.eq(1).val();
                            $progress.fadeOut('fast', function () {
                                $(this).children('div')
                                    .css('width', '0%');
                                $this.removeClass('webuploader-element-invisible');
							});
                        }, 200);
                        //方法回调
                        if ($this.data("uploadsuccess")) {
                        	var funName = $this.data("uploadsuccess"),
                               success = eval("$.fn." + funName)
                            //success = eval(funName);
                            success.call(this, file, response, uploader);
                        }
                    });
                }
            }, 200)

        });
    });
})
