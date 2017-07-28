/*
masy@tunynet.com
2016-10-20
*/

jQuery.fn.sideNav=function () {

    //获取需要点击导航条的外层盒子
    var cascade_box_obj = this;
    //标记导航状态为收回的class名,不能带点或者#号
    var sign_name_class = "tn-narrow-side";
    //切换展开收缩的按钮
    var side_swith_obj = $("#sideSwitch");

    cascade_box_obj.find("a").click(function(){
      if ($("body").hasClass(sign_name_class)) {return};
      if ($(this).siblings("ul").children().length>0) {
        if ($(this).siblings("ul").css("display")=="none") {
          $(this).siblings("ul").slideDown(100);
        }else{
          $(this).siblings("ul").slideUp(100);
        }
      }
    });
    var obj_this;
    //function show_animate(){
    //  obj_this.children("ul").slideDown(100);
    //}

    cascade_box_obj.find("a").parent("li").hover(function(){
      if ($("body").hasClass(sign_name_class)) {
        if ($(this).children("ul").children().length>0) {
          obj_this=$(this);
          obj_this.children("ul").animate({opacity:"1"},300,function(){
            obj_this.children("ul").slideDown(100);
          });
          //var time_animate=setInterval(show_animate,200);
        }
      }
    },function(){
      if ($("body").hasClass(sign_name_class)) {
        if ($(this).children("ul").children().length>0) {
          $(this).children("ul").stop(true,false).slideUp(100);
          //clearInterval(time_animate);
        }
      }
    });

    //todo 当没有页面获取cookie 时启用
    //if (getCookie("sideSwitch")=="1") {
    //    sideSwitch();
    //}

    // switch
    side_swith_obj.on("click", function() {
        sideSwitch();
        return false;
    });

    function setCookie(name,value,iDay){   
        /* iDay 表示过期时间   
        cookie中 = 号表示添加，不是赋值 */   
        var oDate=new Date();   
        oDate.setDate(oDate.getDate()+iDay);       
        document.cookie=name+'='+value+';expires='+oDate;
    }

    function getCookie(name){
        /* 获取浏览器所有cookie将其拆分成数组 */   
        var arr=document.cookie.split('; ');  
        for(var i=0;i<arr.length;i++)    {
            /* 将cookie名称和值拆分进行判断 */       
            var arr2=arr[i].split('=');               
            if(arr2[0]==name){           
                return arr2[1];       
            }   
        }       
        return '';
    }

    //设置导航展开收缩的切换
    function sideSwitch() {
        if ($("#sideSwitch").parents("body").hasClass("tn-narrow-side")) {
            setCookie('sideSwitch',"1",-1);
            $("#sideSwitch").parents("body").removeClass("tn-narrow-side");
            $("#sideSwitch").children("i.fa").removeClass("fa-chevron-circle-right").addClass("fa-chevron-circle-left");
        } else {
            setCookie('sideSwitch',"1",1); //设置键的值
            $("#sideSwitch").parents("body").addClass("tn-narrow-side");
            $("#sideSwitch").children("i.fa").removeClass("fa-chevron-circle-left").addClass("fa-chevron-circle-right");
        };
        //todo @wanglei 重新初始化折叠
        cascade_box_obj.find("a").siblings("ul").slideUp(0).removeClass('selected');
    }

}

