$(document).ready(function() {
    
    //사이드바 메뉴 토글버튼 활성화
    $('[data-toggle="offcanvas"]').click(function() {
        $('.row-offcanvas').toggleClass('active')
    });
    var d = new Date();

    var month = d.getMonth() + 1;
    var day = d.getDate();

    var output = d.getFullYear() + '-' +
        (month < 10 ? '0' : '') + month + '-' +
        (day < 10 ? '0' : '') + day;
    var week;
    if (new Date().getDay() == 0) week = "일"
    if (new Date().getDay() == 1) week = "월"
    if (new Date().getDay() == 2) week = "화"
    if (new Date().getDay() == 3) week = "수"
    if (new Date().getDay() == 4) week = "목"
    if (new Date().getDay() == 5) week = "금"
    if (new Date().getDay() == 6) week = "토"
    $("#realtime").find("#currentdate").html("   "+output + "[" + week + "]");
}); 

function write_question(title_option){
	if(!only_philsu_check()) return false; //필수 항목 체크후 
	if(title_option){
		var message=title_option+" 하시겠습니까?";
		if (confirm(message)){
			$("form").hide();
			open_modal_dialog();//모달 다이얼로그 창	//document.forms[0].submit(); 
		}else{
			return false;
		}
	}else{
		$("form").hide();
		open_modal_dialog();	//모달 다이얼로그 창 //$(".button-group").hide();
	}
}
//삭제여부
function delete_question(){
	var message="삭제 하시겠습니까?";
	if (confirm(message)){
		$('#mode').val('delete');
		$("form").hide();
		open_modal_dialog();
	}else{
		$('#mode').val('update');
		return false;
	}
}
//필수항목체크
function only_philsu_check(){
	var not_checked_count=0;
	$('form .only_philsu').each(function(){
		var check_value=$(this).val().trim();
		if(check_value=="" || check_value==null){
			not_checked_count=1;
			var message="필수 항목을 체크하기 바랍니다.";
			alert(message);
			$(this).focus();
			return false; //each 문을 break로 빠져 나감..
		}
	});
	if(not_checked_count) 	return false; 
	else			 	 	return true;
}

//모달 다이얼로그 창
function open_modal_dialog() {
   
	//Dialog box를 만들기 위한 div 만들기 안쪽 div는 progressbar를 위한것임..
	var dialog_box_div="<div id='gb_dialog-confirm'><div id='gb_progressbar'></div></div>";
    $('body').append(dialog_box_div);
 
    // Dialog 와 속성 정의.
    $("#gb_dialog-confirm").dialog({
		resizable: false,
        modal: true, //뒷배경을 disable 시키고싶다면 true
		title:'Watting..', //dialog 박스의 타이틀..
        //width: 400, //전체 dialog 길이 (기본은 300)
		//height: 'auto', //기본은 auto height: 600,
		minHeight: 100, //내용의 높이 Default: 150 
		open: function(event) {
			//$(".ui-dialog-titlebar").hide();     //타이틀 없애기..
			$('.ui-dialog .ui-dialog-titlebar').css('text-align','center');
			$('.ui-dialog .ui-dialog-titlebar-close').hide(); //close 버튼 없애기..
			submit_form(event);
		}
		
    });
	//progress bar 작동 
	$( "#gb_progressbar" ).progressbar({      
		value: false      
	}); 
}
function submit_form(event){
	
	document.forms[0].submit(); //$('form').submit();
	
	// refress , 78=ctrl+N , 82=ctrl+R ,116= F5,8=backspace 방지
	var script  = document.createElement('script');
  	script.src  = "/zconfig/common_jscss/refresh_prevent.js";
  	script.type = 'text/javascript';
  	script.defer = true;
  	document.getElementsByTagName('head').item(0).appendChild(script);
	
	event.preventDefault();
	$('form').unbind('submit'); // unbind this submit handler first and ...
	$('form').submit(function(){ // added the new submit handler (that does nothing)
		return false;
	});
}


//컴마 넣기
function get_comma(data) {    
	return String(data).replace(/(\d)(?=(?:\d{3})+(?!\d))/g, '$1,');
}
//컴마제거 
function get_delete_comma(data) {
	return String(data).replace(/[^\d\.\-]+/g, '');
}
//컴마제거
function get_uncomma(data) {
	return String(data).replace(/[^\d]+/g, '');
}
//공백제거
function get_delete_space(data) {    
	return String(data).replace(/\s+/g, ''); //공백제거
}



function na_open_window(name, url, left, top, width, height, toolbar, menubar, statusbar, scrollbar, resizable){
  
  pattern = /[-]/gi;	
  name=name.replace(pattern,''); //name에 - 를 없애기
  toolbar_str = toolbar ? 'yes' : 'no';
  menubar_str = menubar ? 'yes' : 'no';
  statusbar_str = statusbar ? 'yes' : 'no';
  scrollbar_str = scrollbar ? 'yes' : 'no';
  resizable_str = resizable ? 'yes' : 'no';
  
  window.open(url, name, 'left='+left+',top='+top+',width='+width+',height='+height+',toolbar='+toolbar_str+',menubar='+menubar_str+',status='+statusbar_str+',scrollbars='+scrollbar_str+',resizable='+resizable_str);
}


//인쇄관련 설정
	
function printPage(p) {
	window.print();
		//chrome = navigator.userAgent.toLowerCase().indexOf('chrome') > -1;
    	//if (chrome) { //google chrome app.Name
        	//window.print();
		//} else if ($.browser.opera) {
			//window.onload = window.print; // helps with Opera
		//} else {
			//window.print();
		//}
}

Date.prototype.formatDate = function (dateType) {
    var date = this;
    if (date >= new Date("2099/12/31")) {
        return "Endless";
    }
    try {
        if (dateType.toLowerCase() == "short") {
            return date.getFullYear() + "/" + ((date.getMonth() + 1).toString().length < 2 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1))
            + "/" + (date.getDate().toString().length < 2 ? "0" + date.getDate() : date.getDate());
        }
        else if (dateType.toLowerCase() == "middle") {
            return date.getFullYear() + "/" + ((date.getMonth() + 1).toString().length < 2 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1))
         + "/" + (date.getDate().toString().length < 2 ? "0" + date.getDate() : date.getDate()) + " " +

             (date.getHours().toString().length < 2 ? "0" + date.getHours() : date.getHours()) + ":"
            + (date.getMinutes().toString().length < 2 ? "0" + date.getMinutes() : date.getMinutes());
        }
        else {
            return date.getFullYear() + "/" + ((date.getMonth() + 1).toString().length < 2 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1))
         + "/" + (date.getDate().toString().length < 2 ? "0" + date.getDate() : date.getDate()) + " " +

             (date.getHours().toString().length < 2 ? "0" + date.getHours() : date.getHours()) + ":"
            + (date.getMinutes().toString().length < 2 ? "0" + date.getMinutes() : date.getMinutes()) + ":"
            + (date.getSeconds().toString().length < 2 ? "0" + date.getSeconds() : date.getSeconds());
        }
    }

    catch (ex) {
        return "N/A";
    }

}

jQuery.fn.serializeJson = function () {
    var serializeObj = {};
    var array = this.serializeArray();
    var str = this.serialize();
    $(array).each(function () {
        if (serializeObj[this.name]) {
            if ($.isArray(serializeObj[this.name])) {
                serializeObj[this.name].push(this.value);
            } else {
                serializeObj[this.name] = [serializeObj[this.name], this.value];
            }
        } else {
            serializeObj[this.name] = this.value;
        }
    });
    return serializeObj;
};
//*************************************//



/*
// 접속 핸드폰 정보 
var userAgent = navigator.userAgent.toLowerCase();
 
// 모바일 홈페이지 바로가기 링크 생성 
if(userAgent.match('iphone')) { 
	document.write('<link rel="apple-touch-icon" href="/zconfig/images/favicons/favicon.png" />') 
} else if(userAgent.match('ipad')) { 
	document.write('<link rel="apple-touch-icon" sizes="72*72" href="/zconfig/images/favicons/favicon.png" />') 
} else if(userAgent.match('ipod')) { 
	document.write('<link rel="apple-touch-icon" href="/zconfig/images/favicons/favicon.png" />')
} else if(userAgent.match('android')) { 
	document.write('<link rel="shortcut icon" href="/zconfig/images/favicons/favicon.png" />') 
}
*/



