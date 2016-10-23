var console = window.console || { log: function() {} }; 
$(window).load(function(){
	$("input[type='password']") .removeClass('ime-kor ime-eng').addClass('ime-eng-only'); 
	

});

$(function(){
	
	//작성자: 김동혁
	if($(".sidebar").is(':visible')) {
		//해당 페이지 문서의 높이에 따라 .content의 min-height 값을 수정한다.
		//footer-wrap의 높이를 구한다
		//console.log("display : block");		//콘솔 메세지를 통해 css가 제대로 적용 됐는지 확인
		var footerHeight = $(".footer-wrap").height();
		//header의 높이 만큼 빼준다. 여기서 selector가 this라면 body의 현재 높이를 의미하고, document라면 html 문서의 높이값을 의미한다.
		//var minHeight = $(document).height() - 50 - footerHeight;	//빈공간 생겨서 아래 수식으로 변경함.
		var minHeight = $(document).height()-70 - footerHeight + 20;
		$(".content-wrap .content").css({"minHeight":minHeight+"px"});
	} else {
		//console.log("display : none");		//콘솔 메세지를 통해 css가 제대로 적용 됐는지 확인
	    var minHeight = $(document).height() - 50 - 130;
		$(".content-wrap .content").css({"minHeight":minHeight+"px"});
	}


	//라운딩 처리..테투리 색깔 지정..
	$("input[type='text'],input[type='search'], input[type='password'], select").addClass('form-control'); 
	
	
	//view 상단의 모든 select 의 길이 조정
	$(".panel-body select").each(function(){ //모든 select박스를 조사
		var text_length=$(this).children('option:eq(0)').text().length; //각 select의 첫번째 option의 text값의 길이
		//길이에따른 width값 조정
		switch (text_length) {
			case 1:      $(this).css('width','40px');	break;
			case 2:      $(this).css('width','55px');	break;
			case 3:      $(this).css('width','65px');	break;
			case 4:      $(this).css('width','80px');	break;  
			case 5:      $(this).css('width','90px');	break;
			case 6:      $(this).css('width','105px');	break;
			case 7:      $(this).css('width','115px');	break;
			default:		 $(this).css('width','auto');	break;	
		}
	});
	
	//focus 혹은  blur시 색상 변경 및 양끝 공백제거
	$("input[type='text'],input[type='search'], input[type='password'], input[type='file'], textarea,select").on({ 
		focus  : function() { 
			var readonly=$(this).prop('readonly');
		 	if(!readonly) 	$(this).css("background-color","#bef2ff");
		},
		blur: function(){  
			 var readonly=$(this).prop('readonly');
			if(!readonly) $(this).css("background-color","");
		},
		change:function(){ //trim으로 양끝 공백제거
			var cur_tagName=$(this).prop('tagName').toUpperCase(); /*현재 tag의 name*/
			/*select 인 경우는 제외 왜냐면 이부분을 적용하면 select multiple 인 경우 여러개를 선택할수 없음*/
			if(!(cur_tagName=='SELECT' || cur_tagName=='TEXTAREA')){ 
				var data=$.trim($(this).val()); 
				if(data != null && data != '')	$(this).val(data);
			}
		}
	});
	
	$(".only_tooltip").tooltip({ 
		html: true,
		trigger: "hover",
		/*tooltip 위치 지정 없애면 기본은 top*/
		placement: function(tip, element) { //위치 $this is implicit
			var position = $(element).position();
			//if (position.left > 100) { return "left"; }
			//if (position.left < 100) { return "right"; }
			if (position.top < 50)		return "bottom";	
			else								return "top";
		}

	}).click(function () {
		$(this).tooltip('hide');
	}); //툴팁 bootstrap 용 jquery UI가 아님
	//$('.only_tooltip_link').tooltip({ trigger: "hover"}); 
	
	$('.only_popover').popover({ 
		html : true, 
		placement: 'bottom',
		//trigger: "hover",
		content: function() {
			return $(this).attr('popover-content');
		},
		title: function(){
			var popover_title=$(this).attr('popover-title');
			if(popover_title)	return '<span class="p-title">'+popover_title+'</span>'+'<span class="close">&times;</span>';
			else						return "-"+'<span class="close">&times;</span>';
		}
	}).on('shown.bs.popover', function(e){
		//
		//var w = $(this).attr("data-max-width");
		/*창의 크기 지정*/
		/*
 	   	 $(this).data('bs.popover').tip().css({
			'max-width': '600px',
			'min-width': '400px'
		});
		*/
		/*close 버튼 클릭시 닫기*/
		var popover = $(this);
		$(this).parent().find('div.popover .close').on('click', function(e){
			popover.popover('hide');
		});
	}).click(function (e) {
		$('.only_popover').each(function () {
			//the 'is' for buttons that trigger popups
			//the 'has' for icons within a button that triggers a popup
			if (!$(this).is(e.target) && $(this).has(e.target).length === 0 && $('.popover').has(e.target).length === 0) {
				$(this).popover('hide');
			}
		});
	});
	
	//바탕화면 클릭시 only_popover 없어지기..
	$('body').on('click', function (e) {
		$('.only_popover').each(function () {
			//the 'is' for buttons that trigger popups
			//the 'has' for icons within a button that triggers a popup
			if (!$(this).is(e.target) && $(this).has(e.target).length === 0 && $('.popover').has(e.target).length === 0) {
				$(this).popover('hide');
			}
		});
	});
	
	/**
	이미지 풍선 도움말 소스: http://javaking75.blog.me/220254435144
	common.css 의 image_tooltip과 관련
	**/
	var image_path_temp; //이미지 경로
	var image_path; //이미지 경로
	var image_temp; // type이 img로 정해질 경우 얻어지는 <img> 태그의 내용
	$(".only_img_tooltip").hover(
		function(){
			image_path = $(this).prop("title");		//이미지 경로
			//image_path=image_path_temp.substring(2);
			 $(this).attr("title",""); //브라우저가 title속성의 값을 기본적 툴팁으로 보여주기 때문에 방지를 위해 값을 공백으로 처리
			image_temp = "<img src='"+image_path+"'>";
			$("body").append("<div id='image_tooltip'></div>");//내용을 넣을 영역 생성
			$("#image_tooltip").html(image_temp);
			$("#image_tooltip").css({'width':'152px','height':'152px'});
			var img = new Image(); 
			img.src = image_path ; 
			$("#image_tooltip img").load(function () { //이미지가 로딩이 완료 된 후 
				var w	= img.width;	//원본 이미지 가로길이
				var h	= img.height;	//원본 이미지 세로길이

				if(w > h){//이미지의 세로가 길 경우 css text-align:center;로 가운데 정렬이 되나 세로가 길 경우에는 top:50%, margin-top:리사이징된 이미지 높이 1/2 이 되어야 한다.
					$("#image_tooltip img").css({'width':'100%','height':'auto','top':'50%'});	//원본 이미지를 150X150 DIV 안에 담는다.
					var sh2 = $("#image_tooltip img").height() / 2 + 1;	//리사이징된 이미지 높이 1/2 (+1px은 오차 수정)
					$("#image_tooltip img").css({'marginTop':'-'+sh2+'px'});	//margin-top의 값을 새로 준다.
				}
				else if(h > w)	$("#image_tooltip img").css({'width':'auto','height':'100%'});
				else					$("#image_tooltip img").css({'width':'100%','height':'100%'});
			});

		    //도움말영역이 나타날 위치 구하기
			var pagex = $(this).offset().left - 20;
			var pagey = $(this).offset().top - 10-$("#image_tooltip").innerHeight();
			$("#image_tooltip").css({left:pagex+"px", top:pagey+"px"}).fadeIn(50);
		},
		function(){
			$(this).prop("title",image_path);				
			$("#image_tooltip").remove(); //마우스커서가 떠나면 툴팁영역 제거
		}
	);

	//포커스 이동
	$('.only_focus').on('keypress keydown', function(event) {
		var el=$('.only_focus');
		var key = get_key_code();
		/**뒤로 이동**/
		if(event.shiftKey &&  key == 9){ //Shift+TAb 동시키 뒤로 이동	 
			var cur_tagName=$(this).prop('tagName').toUpperCase();
			if(cur_tagName!='TEXTAREA'){
				event.preventDefault();
				var cur_index	=$(el).index(this); 	//현재index
				if(cur_index){ //처음이라면 아니라면
					var pre_index 	= cur_index - 1; 	//현재index의 이전 index
					var next_tagName=$(el).eq(pre_index).prop('tagName').toUpperCase(); //type의 종류
					if(next_tagName=='INPUT')	$(el).eq(pre_index).focus().select();
					else										$(el).eq(pre_index).focus();	
				}else{ //처음이라면 제자리에 머물기
					return false;
				}
			}
		/**앞으로 이동**/	
		}else if (key == 13 || key == 9) { //keypress 시 13=enter, keydown시	 TAb =9 
			var cur_tagName=$(this).prop('tagName').toUpperCase();
			if(cur_tagName!='TEXTAREA'){
				event.preventDefault();
				if($(this).hasClass('only_philsu') ||  $(this).hasClass('only_philsu_each') ){ //팰수필드인데 값을 가지고 있지 않다면..
					if(!$(this).val()) return false;
				}
				var cur_index	=$(el).index(this); 	//현재index
				var total_length = $(el).length; 		//총 개수
				var pre_index 	= cur_index - 1; 	//현재index의 이전 index
				var next_index 	= cur_index + 1;	//현재index의 다음 index
				if (next_index == total_length) { //이동할 index가 마지막이라면 이전index로 이동
					$('button[name=button_write]').eq(0).focus();
				}else{ //마지막index가 아니라면..다음index로 포커스 이동
					var next_tagName=$(el).eq(next_index).prop('tagName').toUpperCase(); //type의 종류
					if(next_tagName=='INPUT')	$(el).eq(next_index).focus().select();
					else						$(el).eq(next_index).focus();	
				}
			}
		}

	});

	//모든공백제거
	$(".only_delete_space").on({ 
		change:function(){ 
			var data=$(this).val().replace(/\s+/g,"");
			if(data != null && data != '')	$(this).val(data);
		}
	});	
	//공백제거 후 대문자.
	$(".only_delete_space_upper").on({ 
		change:function(){ 
			var data=$(this).val().replace(/\s+/g,"").toUpperCase();
			if(data != null && data != '')	$(this).val(data);
		}
	});	

	//정수만 입력가능 (- 가능)
	$(".only_number").on({ 
		focus:		function(){	$(this).removeClass('ime-kor ime-eng').addClass('ime-eng-only'); },
		keypress:	function(){	input_keypress_check('number');			},
		blur:		function(){
			var data=$(this).val().replace(/\s+/g,"");
			if(data != null && data != ''){
				var pattern = /^(-?)([0-9]*)([^0-9]*)([0-9]*)([^0-9]*)/;
				if(!pattern.test(data)){
					alert("양수 혹은 음수만 가능합니다.");	$(this).val("").focus(); return false;
				}
				data = data.replace(pattern, "$1$2$4"); 	$(this).val(data);//인풋 박스에 삽입 
			}
		}
	});	
	//양의 정수만 입력가능
	$(".only_number_positive").on({ 
		focus:		function(){	$(this).removeClass('ime-kor ime-eng').addClass('ime-eng-only'); },
		keypress:	function(){	input_keypress_check('number_positive');	},
		blur:		function(){
			var data=$(this).val().replace(/\s+/g,"");
			data=parseInt(Math.abs(data),10); //절대값으로 환산해 본다..
			if(data != null && data != ''){
				var pattern = /^[0-9]+$/;
				if(!pattern.test(data)){
					alert("양의 정수만 가능 합니다.");	$(this).val("").focus(); return false;
				}
			}
		}
	});

	//실수까지 입력가능(-포함)
	$(".only_decimal").on({ 
		focus:		function(){	$(this)	.removeClass('ime-kor ime-eng').addClass('ime-eng-only'); },
		keypress:	function(){	input_keypress_check('decimal');			},
		blur:		function(){
			var data=$(this).val().replace(/\s+/g,"");
			if(data != null && data != ''){
				var pattern=/^(-?)([0-9]*)(\.?)([^0-9]*)([0-9]*)([^0-9]*)/;
				if(!pattern.test(data)){
					alert("양의 실수 혹은 음의실수만 가능합니다.");	$(this).val("").focus(); return false;
				}
				data = data.replace(pattern, "$1$2$3$5"); 	$(this).val(data);//인풋 박스에 삽입 
			}
		}//blur
	});
	//양의 실수만 입력가능
	$(".only_decimal_positive").on({ 
		focus:		function(){	$(this)	.removeClass('ime-kor ime-eng').addClass('ime-eng-only'); },
		keypress:	function(){	input_keypress_check('decimal_positive');	},
		blur:		function(){
			var data=$(this).val().replace(/\s+/g,"");
			if(data != null && data != ''){
				var pattern=/^(-?)([0-9]*)(\.?)([^0-9]*)([0-9]*)([^0-9]*)/;
				if(!pattern.test(data)){
					alert("양의 실수만 가능합니다.");	$(this).val("").focus(); return false;
				}
				data = data.replace(pattern, "$2$3$5"); 	$(this).val(data);//인풋 박스에 삽입 
			}
		}
	});



	//국내 전화번호및 핸드폰번호만 입력
	$(".only_phone").on({ 
		focus:		function(){	$(this)	.removeClass('ime-kor ime-eng').addClass('ime-eng-only'); },
		keypress:	function(){	input_keypress_check('number');	},
		blur:		function(){
			var data_check=$(this).val().replace(/\s+/g,""); //공백제거
			if(data_check != null && data_check != ''){
				var data = $(this).val().replace(/[^\d]/gi,''); //숫자를 제외한 모든것제거...
				var data_length=data.length;
				if(data_length < 5 || data_length > 11) { //자리수 체크
					alert("전화번호는 5~11자리 입니다."); 	  $(this).val('').focus().select(); return false;
				}
				
				var pattern = "";
				var replace_pattern="";
				pattern = /^01[0-9]|0[5-9]0/gi; //앞3자리수가 핸드폰 번호 혹은,070,080이라면 
				if(pattern.test(data.substr(0,3)) && data_length < 10 ){
					alert("전화번호 체계가 이상합니다.");   
					$(this).focus().select(); return false;
				}
				switch (data_length) {
					case 5	: replace_pattern=/^([0-9]{2})-?([0-9]{3})$/gi;							data = data.replace(replace_pattern, "$1-$2"); 	break;
					case 6	: replace_pattern=/^([0-9]{3})-?([0-9]{3})$/gi;							data = data.replace(replace_pattern, "$1-$2");	break;
					case 7	: replace_pattern=/^([0-9]{3})-?([0-9]{4})$/gi;							data = data.replace(replace_pattern, "$1-$2");	break;
					case 8	: replace_pattern=/^([0-9]{4})-?([0-9]{4})$/gi;							data = data.replace(replace_pattern, "$1-$2");	break;
					case 9	: replace_pattern=/^([0-9]{2})-?([0-9]{3})-?([0-9]{4})$/gi;				data = data.replace(replace_pattern, "$1-$2-$3");	break;
					default: replace_pattern= /(^02.{0}|^01.{1}|[0-9]{3})([0-9]+)([0-9]{4})$/gi;	data = data.replace(replace_pattern, "$1-$2-$3");	break;
				}
				$(this).val(data);//인풋 박스에 삽입 
			}
		}//blur
	});

	//국내 핸드폰 번호만 입력
	$(".only_mobile").on({ 
		focus:		function(){	$(this)	.removeClass('ime-kor ime-eng').addClass('ime-eng-only'); },
		keypress:	function(){	input_keypress_check('number');	},
		blur:		function(){
			var data_check=$(this).val().replace(/\s+/g,""); //공백제거
			if(data_check != null && data_check != ''){
				
				var data = $(this).val().replace(/[^\d]/gi,''); //숫자를 제외한 모든것제거...
				if(data.length < 10 || data.length > 11) { //휴대폰 번호는 11 혹은 10 자리 
					alert("핸드폰번호는 10~11자 이어야합니다.");	  $(this).val('').focus().select(); return false;
				}
				var pattern = /^01([016789])([1-9]{1})([0-9]{2,3})([0-9]{4})$/gi; //휴대폰 번호가 유효한 번호인지 정규식으로 검사
				if(!pattern.test(data)){
					alert("핸드폰 체계가 이상합니다.");	  $(this).focus().select(); return false;
				}
				var replace_pattern=/^(01[016789]{1}|02|0[3-9]{1}[0-9]{1})-?([0-9]{3,4})-?([0-9]{4})$/gi;
				data = data.replace(replace_pattern, "$1-$2-$3"); //정상적인 번호라면 01012341234 => 010-1234-1234 의 형태로 변경
				$(this).val(data);//인풋 박스에 삽입 
			}
		}//blur
	});


	//날짜형식만 입력
	$(".only_date").on({
		focus:		function(){	$(this)	.removeClass('ime-kor ime-eng').addClass('ime-eng-only'); },
		keypress:	function(){	input_keypress_check('number');	},
		change:		function(){
			var data_check=$(this).val().replace(/\s+/g,""); //공백제거
			if(data_check != null && data_check != ''){
				var data = $(this).val().replace(/[^\d]/gi,''); //숫자를 제외한 모든것제거...
				if(data.length!=8 ) { //데이터 길이 체크
					alert("일자를 체크하기 바랍니다.\n예: YYYY-MM-DD 혹은 YYYYMMDD");	 
					$(this).val('').focus().select(); 
					return false;
				}
				var class_index=$('.only_date').index(this); //클래스의 index
				var target_calss_name=$(this).prop('class');
				
				//$.ajax({
				//	type:'POST',
				//	url:'/zconfig/common_php/loadData_check_date.php',
				//	data : { 
				//		date:data, //일자
				//		class_index:class_index, //타켓아이디
				//		target_calss_name:target_calss_name  //클래스 이름: calendar_max,calendar_min를 구분하여 범위 설정하기..
				//	},
				//	dataType:'script',
				//	success:function(data,script){
				//		//eval(script);
				//	}
				//});
				
			}			
		} //blur
	});
	//우편번호만 입력
	$(".only_zipcode").on({ 
		focus:		function(){	$(this)	.removeClass('ime-kor ime-eng').addClass('ime-eng-only'); },
		keypress:	function(){	input_keypress_check('number');	},
		blur:		function(){
			
			//$(this).val(data);
			var data_check=$(this).val().replace(/\s+/g,""); //공백제거
			if(data_check != null && data_check != ''){
				var data = $(this).val().replace(/[^\d]/gi,''); //숫자를 제외한 모든것제거...
				if(data.length!=6 ) { //데이터 길이 체크
					alert("우편번호 자리수는 6자리입니다. 체크하기 바랍니다.\n예: 123-456 혹은 123456");	  $(this).val('').focus().select(); return false;
				}
				 var replace_pattern=/^([0-9]{3})-?([0-9]{3})$/gi;
				 data = data.replace(replace_pattern, "$1-$2"); //정상적인 번호라면 123456 => 123-456 의 형태로 변경
				 $(this).val(data);//인풋 박스에 삽입 
			}
		}//blur
	});
	
	//이메일
	$(".only_email").on({ 
		focus:function(){	$(this).removeClass('ime-kor ime-eng-only').addClass('ime-eng');	},
		blur:		function(){
			var data=$(this).val().replace(/\s+/g,"");//공백제거
			if(data != null && data != ''){
				$(this).val(data);
				var pattern=/^[0-9a-zA-Z가-힣]([-_\.]?[0-9a-zA-Z가-힣])*@[0-9a-zA-Z가-힣]([-_\.]?[0-9a-zA-Z가-힣])*\.[a-zA-Z가-힣]{1,17}$/gi;
				if(!pattern.test(data)){
					alert("이메일을 체크하기 바랍니다.\n예: abc@goldpen.co.kr");	
					 $(this).focus().select(); return false;
				}
			}
		}//blur
	});

	//도메인 입력
	$(".only_domain").on({ 
		focus:function(){	$(this).removeClass('ime-kor ime-eng-only').addClass('ime-eng');	},
		blur:		function(){
			var data=$(this).val().replace(/\s+/g,"");
			//
			if(data != null && data != ''){
				$(this).val(data);
				//참조 http://dvlp.tistory.com/219
				var pattern=/^((https?|ftp):\/\/)?((([a-z\d](([a-z\d-]*[a-z\d]))|([ㄱ-힣])*)\.)+(([a-zㄱ-힣]{2,}))|((\d{1,3}\.){3}\d{1,3}))(\:\d+)?(\/[-a-z\d%_.~+]*)*(\?[;&a-z\d%_.~+=-]*)?(\#[-a-z\d_]*)?$/gi;
				if(!pattern.test(data)){
					alert("도메인을 체크하기 바랍니다.\n예: \n http://abc.co.kr \n https://abc.co.kr \n ftp://abc.com ");	
					 $(this).focus().select(); return false;
				}
			}
		}//blur
	});

	//사업자번호 입력
	$(".only_reg_no").on({ 
		focus:		function(){	$(this)	.removeClass('ime-kor ime-eng').addClass('ime-eng-only'); },
		keypress:	function(){	input_keypress_check('number');	},
		blur:		function(){
			
			//$(this).val(data);
			var data_check=$(this).val().replace(/\s+/g,""); //공백제거
			if(data_check != null && data_check != ''){
				var data = $(this).val().replace(/[^\d]/gi,''); //숫자를 제외한 모든것제거...
				if(data.length!=10 ) { //데이터 길이 체크
					alert("사업자번호 길이는 10자리이어야합니다.\n예: 123-45-67890 혹은 1234567890");	  
					$(this).val('').focus().select();		return false;
				}
				//규칙체크  참조사이트 http://hatssarjy.tistory.com/246
				var checkID = new Array(1, 3, 7, 1, 3, 7, 1, 3, 5, 1); 
				var tmpBizID, i, chkSum=0, c2, remander; 
				
				 for (i=0; i<=7; i++) chkSum += checkID[i] * data.charAt(i); 
				 c2 = "0" + (checkID[8] * data.charAt(8)); 
				 c2 = c2.substring(c2.length - 2, c2.length); 
				 chkSum += Math.floor(c2.charAt(0)) + Math.floor(c2.charAt(1)); 
				 remander = (10 - (chkSum % 10)) % 10 ; 
				if (Math.floor(data.charAt(9)) != remander){
					alert("사업자번호가 유효하지 않습니다.");	 $(this).focus().select(); return false;
				}
				var replace_pattern=/^([0-9]{3})-?([0-9]{2})-?([0-9]{5})$/gi;
				data = data.replace(replace_pattern, "$1-$2-$3"); //정상적인 번호라면 1234567890 => 123-45-67890 의 형태로 변경
				$(this).val(data);//인풋 박스에 삽입 
			}
		}//blur
	});
	
	//법인등록번호 입력
	$(".only_reg_legal_no").on({ 
		focus:		function(){	$(this)	.removeClass('ime-kor ime-eng').addClass('ime-eng-only'); },
		keypress:	function(){	input_keypress_check('number');	},
		blur:		function(){
			var data_check=$(this).val().replace(/\s+/g,""); //공백제거
			if(data_check != null && data_check != ''){
				var data = $(this).val().replace(/[^\d]/gi,''); //숫자를 제외한 모든것제거...
				if(data.length!=13 ) { //데이터 길이 체크
					alert("법인등록번호는 13자리이어야 합니다.\n예: 123456-1234567 혹은 1234561234567");	  
					$(this).val('').focus().select(); 	return false;
				}
				//규칙체크 참조사이트 http://hatssarjy.tistory.com/246
				var arr_regno  = data.split("");
				var arr_wt   = new Array(1,2,1,2,1,2,1,2,1,2,1,2);
				var iSum_regno  = 0;
				var iCheck_digit = 0;
				for (i = 0; i < 12; i++){
				 iSum_regno +=  eval(arr_regno[i]) * eval(arr_wt[i]);
				}
				iCheck_digit = 10 - (iSum_regno % 10);
				iCheck_digit = iCheck_digit % 10;
				if (iCheck_digit != arr_regno[12]){
					alert("법인번호가 유효하지 않습니다.");	 $(this).focus().select(); 	return false;
				}
				
				var replace_pattern=/^([0-9]{6})-?([0-9]{7})$/gi;
				data = data.replace(replace_pattern, "$1-$2"); //정상적인 번호라면 1234561234567 => 123456-1234567 의 형태로 변경
				$(this).val(data);//인풋 박스에 삽입 
			}
		}//blur
	});

	//주민번호 입력 체크
	$(".only_jumin_no").on({ 
		focus:		function(){	$(this)	.removeClass('ime-kor ime-eng').addClass('ime-eng-only'); },
		keypress:	function(){	input_keypress_check('number');	},
		blur:		function(){
			var data_check=$(this).val().replace(/\s+/g,""); //공백제거
			if(data_check != null && data_check != ''){
				var data = $(this).val().replace(/[^\d]/gi,''); //숫자를 제외한 모든것제거...
				if(data.length!=13 ) { //데이터 길이 체크
					alert("주민등록번호는 13자리이어야 합니다.\n예: 123456-1234567 혹은 1234561234567");	 
					$(this).val('').focus().select(); return false;
				}
				 var replace_pattern=/^([0-9]{6})-?([0-9]{7})$/gi;
				 var data_temp = data.replace(replace_pattern, "$1-$2"); //정상적인 번호라면 1234561234567 => 123456-1234567 의 형태로 변경
				 $(this).val(data_temp);//인풋 박스에 삽입 
					
				 //규칙체크 참조사이트 http://hatssarjy.tistory.com/246
				 if(data_temp.match(/^\d{2}[0-1]\d[0-3]\d-[1-4]\d{6}$/) == null) {
					alert("주민등록번호가 유효하지 않습니다.");	 $(this).focus().select(); 	return false;
				  }
				  var chk = 0;
				  var i;
				  var last_num = data_temp.substring(13, 14);
				  var chk_num = '234567-892345';
				  for(i = 0; i < 13; i++) {
					if(data_temp.charAt(i) != '-')	chk += ( parseInt(chk_num.charAt(i)) * parseInt(data_temp.charAt(i)) );
				  }
				 chk = (11 - (chk % 11)) % 10;
				 if (chk != last_num){
					alert("주민등록번호가 유효하지 않습니다.");	 $(this).focus().select(); return false;
				 }
			}
		}//blur
	});
	
	//id체크
	$(".only_id").on({ 
		focus:function(){	$(this).removeClass('ime-kor ime-eng').addClass('ime-eng-only');	},
		blur:		function(){
			var data=$(this).val().replace(/\s+/g,""); //공백제거
			if(data != null && data != ''){
				//길이  체크..
				if((data.length < 4)  || (data.length > 20)) {
					alert("아이디는 길이는 4~20자 이어야 합니다.");	  
					 $(this).focus().select(); return false;
				}
				var pattern = /^[a-z]+[a-z0-9-_]{3,19}$/gi;
				if(!pattern.test(data)){
					alert('아이디 첫 글자는 영문으로 시작해야합니다.\n영문,숫자,언더바(_),하이픈(-)만 사용할 수 있습니다');
					 $(this).focus().select(); return false;
				}
				$(this).val(data.toLowerCase()); //소문자로 변경..
			}
		}
	});
	
	//패스워드 체크
	$(".only_password").on({ 
		focus:function(){	$(this).removeClass('ime-kor ime-eng').addClass('ime-eng-only');	},
		change:		function(){
			var data=$(this).val().replace(/\s+/g,"");
			if(data != null && data != ''){
				if(data.length < 4) {
					 alert("패스워드 길이는 4자이상 이어야 합니다.");	  
					 $(this).val("").focus().select(); return false;
				}
				//var pattern=	 /^[A-za-z0-9!@#$%^*()\-_=+\\\|\[\]{};:\'",.<>\/?]/g; //영문 혹은 숫자 혹은 ? & \' \" + 을 제외한 특수문자만 사용할 수 있습니다
				//if(!pattern.test(data)){
					//alert('영문,숫자 혹은 키보드에 있는 특수문자(&amp; 제외)만 사용할 수 있습니다');
					 //$(this).val('').focus().select(); return false;
				//}
			}
		}
	});

	//각종 패턴 체크 후...메시지 뿌리기
	$('.only_HanEngNumNomal').blur(function()	{	var input_name=$(this).attr('name'); var i=$("input[name='"+input_name+"']").index(this); 	var data=$("input[name='"+input_name+"']").eq(i).val();	if(data != null && data != '')	input_data_pattern_check('HanEngNumNomal',data,input_name,i);		});
	$('.only_HanEngNum').blur(function()		{	var input_name=$(this).attr('name'); var i=$("input[name='"+input_name+"']").index(this);  	var data=$("input[name='"+input_name+"']").eq(i).val(); 	if(data != null && data != '')	input_data_pattern_check('HanEngNum',data,input_name,i);			});
	$('.only_HanEng').blur(function()			{	var input_name=$(this).attr('name'); var i=$("input[name='"+input_name+"']").index(this); 	var data=$("input[name='"+input_name+"']").eq(i).val();	if(data != null && data != '')	input_data_pattern_check('HanEng',data,input_name,i);				});
	$('.only_HanNum').blur(function()			{	var input_name=$(this).attr('name'); var i=$("input[name='"+input_name+"']").index(this); 	var data=$("input[name='"+input_name+"']").eq(i).val();	if(data != null && data != '')	input_data_pattern_check('HanNum',data,input_name,i);				});
	$('.only_EngNum').blur(function()			{	var input_name=$(this).attr('name'); var i=$("input[name='"+input_name+"']").index(this); 	var data=$("input[name='"+input_name+"']").eq(i).val();	if(data != null && data != '')	input_data_pattern_check('EngNum',data,input_name,i);				});
	$('.only_Han').blur(function()				{	var input_name=$(this).attr('name'); var i=$("input[name='"+input_name+"']").index(this); 	var data=$("input[name='"+input_name+"']").eq(i).val();	if(data != null && data != '')	input_data_pattern_check('Han',data,input_name,i);				});
	$('.only_Eng').blur(function()				{	var input_name=$(this).attr('name'); var i=$("input[name='"+input_name+"']").index(this);	var data=$("input[name='"+input_name+"']").eq(i).val();	if(data != null && data != '')	input_data_pattern_check('Eng',data,input_name,i);				});
	$('.only_Num').blur(function()				{	var input_name=$(this).attr('name'); var i=$("input[name='"+input_name+"']").index(this);	var data=$("input[name='"+input_name+"']").eq(i).val();	if(data != null && data != '')	input_data_pattern_check('Num',data,input_name,i);				});
	$('.only_NumPositive').blur(function()		{	var input_name=$(this).attr('name'); var i=$("input[name='"+input_name+"']").index(this);	var data=$("input[name='"+input_name+"']").eq(i).val();	if(data != null && data != '')	input_data_pattern_check('NumPositive',data,input_name,i);		});	
	$('.only_Decimal').blur(function()			{	var input_name=$(this).attr('name'); var i=$("input[name='"+input_name+"']").index(this);	var data=$("input[name='"+input_name+"']").eq(i).val();	if(data != null && data != '')	input_data_pattern_check('Decimal',data,input_name,i);			});
	$('.only_DecimalPositive').blur(function(){	var input_name=$(this).attr('name'); var i=$("input[name='"+input_name+"']").index(this);	var data=$("input[name='"+input_name+"']").eq(i).val();	if(data != null && data != '')	input_data_pattern_check('DecimalPositive',data,input_name,i);	});		
	
	//붙여넣기는 못하게 한다.
	$('.no_paste').bind("paste",function(e) {
		 e.preventDefault();
	});
	//자르기, 카피, 붙여넣기 금지
	$('.no_cut_copy_paste').bind("cut copy paste",function(e) {
		 e.preventDefault();
	});

	
	
	// http://www.decorplanit.com/plugin/참조
	//$('.autoNumeric').autoNumeric('init'); //수동으로 조정..   

	$('.only_auto_comma_default').autoNumeric('init'); 					//표준값 
	$('.only_auto_number').autoNumeric('init',{mDec:'0',aSep:''}); 					//숫자만 
	$('.only_auto_comma_max_1').autoNumeric('init',{vMax:'1',mDec:'0'});			//최대값 10
	$('.only_auto_comma_max_10').autoNumeric('init',{vMax:'10',mDec:'0'});			//최대값 10
	$('.only_auto_comma_max_100').autoNumeric('init',{vMax:'100',mDec:'0'});			//최대값 100
	$('.only_auto_comma_max_1000').autoNumeric('init',{vMax:'1000',mDec:'0'});			//최대값 1000

	$('.only_auto_comma').autoNumeric('init',{vMin:'-2100000000',vMax:'2100000000',mDec:'0'}); 					//정수
	$('.only_auto_comma_big').autoNumeric('init',{vMin:'-9223372036854775808',vMax:'9223372036854775807',mDec:'0'}); 					//정수
	$('.only_auto_comma_positive').autoNumeric('init',{vMin:'0',vMax:'2100000000',mDec:'0'}); //양의정수
	//실수
	$('.only_auto_float')  .autoNumeric('init',{vMin:'-2100000000',vMax:'2100000000',mDec:'2'});
	$('.only_auto_float_1').autoNumeric('init',{vMin:'-2100000000',vMax:'2100000000',mDec:'1'});
	$('.only_auto_float_2').autoNumeric('init',{vMin:'-2100000000',vMax:'2100000000',mDec:'2'});

	$('.only_auto_float_3').autoNumeric('init',{vMin:'-2100000000',vMax:'2100000000',mDec:'3'});
	$('.only_auto_float_4').autoNumeric('init',{vMin:'-2100000000',vMax:'2100000000',mDec:'4'});
	$('.only_auto_float_5').autoNumeric('init',{vMin:'-2100000000',vMax:'2100000000',mDec:'5'});
	
	//양의 실수만
	$('.only_auto_float_positive')  .autoNumeric('init',{vMin:'0',vMax:'2100000000',mDec:'2'});
	$('.only_auto_float_positive_1').autoNumeric('init',{vMin:'0',vMax:'2100000000',mDec:'1'});
	$('.only_auto_float_positive_2')  .autoNumeric('init',{vMin:'0',vMax:'2100000000',mDec:'2'});
	$('.only_auto_float_positive_3').autoNumeric('init',{vMin:'0',vMax:'2100000000',mDec:'3'});
	$('.only_auto_float_positive_4').autoNumeric('init',{vMin:'0',vMax:'2100000000',mDec:'4'});
	$('.only_auto_float_positive_5').autoNumeric('init',{vMin:'0',vMax:'2100000000',mDec:'5'});

	//0이면..비운다..
	$(".only_auto_comma_max_1 ,.only_auto_comma_max_10, .only_auto_comma_max_100, .only_auto_comma_max_1000").on({ 
		blur:function(){
			pattern = /[^\d\-\.]/gi; //-와 숫자를 제외한 모든것 삭제	
			var data=parseInt($(this).val().replace(pattern,''),10);
			if(!data) $(this).val('');
		}
	});
	//0이면..비운다..
	$(".only_auto_comma ,.only_auto_comma_big, .only_auto_comma_positive, .only_auto_number").on({ 
		blur:function(){
			pattern = /[^\d\-\.]/gi; //-와 숫자를 제외한 모든것 삭제	
			var data=parseInt($(this).val().replace(pattern,''),10);
			if(!data) $(this).val('');
		}
	});


	//0이면..비운다..
	$(".only_auto_float   , .only_auto_float_1	  ,.only_auto_float_2  , .only_auto_float_3	  , .only_auto_float_4	  , .only_auto_float_5").on({ 
		blur:function(){
			pattern = /[^\d\.\-]/gi; 	
			var data=parseFloat($(this).val().replace(pattern,''));
			if(!data) $(this).val('');
		}
	});
	$(".only_auto_float_positive , .only_auto_float_positive_1 , .only_auto_float_positive_2  , .only_auto_float_positive_3	 , .only_auto_float_positive_4 , .only_auto_float_positive_5").on({ 
		blur:function(){
			pattern = /[^\d\.\-]/gi;	
			var data=parseFloat($(this).val().replace(pattern,''));
			if(!data) $(this).val('');
		}
	});

	/*
	체크박스 클릭시 라인배경색상 바꾸기 bg-gray 사용자 정의 class임 bootstrap에 있는 success ,waring , info 도 사용가능.
	checkbox_check_all() 함수도 참조할것.
	*/
	$("table input:checkbox[name='my_check']").click(function (){
		 $(this).closest('tr').toggleClass("bg-gray", this.checked);
		  //$(this).closest('tr').css({"color": "red"}, this.checked);
	 });
	
	/*select2 플러그인을 이용한 select박스 검색 select box 검색을 위한 plugin (https://select2.github.io/ 참조) */
	$(".only_select2_search").select2({
		language: "ko",
		//placeholder: "XXXXX", /* 직접하고 싶다면 data-placeholder='xXXX' */
		//tags: "true", /*일치되지 않아도 입력가능*/
		allowClear: true
	});
});


//키코드값알아내기
function get_key_code(){
	/***
	?... :... 연산자는 if...else... 문장의 단축문장으로, 일반적으로 사용하는 긴 if...else... 문장을 간단하게 하기 위하여 사용한다.
	쉽게 말하면 검정문이 맞으면 수행문1을 수행하고 틀리면 수행문2를 수행한다.
	따라서 조건에 관련된 변수값에 따라서 결과가 다르게 나오게 되는 경우에 주로 사용된다.
	****/
	//브라우저마다...키이벤트 값이 틀려서..
	//var keyCode = window.event.keyCode ? window.event.keyCode : window.event.which ? window.event.which : window.event.charCode;
	var keyCode = event.keyCode ? event.keyCode : event.which ? event.which : event.charCode; 
	return keyCode;
}
//keypress시 입력제한
function input_keypress_check(option){
	//48-57은 숫자, 45는 "-" 기호 46은 "."
	var keyCode=get_key_code();
   	//var keyCode = event.keyCode ? event.keyCode : event.which ? event.which : event.charCode; 
	//console.log(keyCode);
	switch (option) {
   		case 'number' 			:	if((keyCode < 48 || keyCode >57 ) && (keyCode != 45)) 								event.returnValue=false;	break;
		case 'number_positive'	:	if( keyCode < 48 || keyCode > 57)													event.returnValue=false;	break;
		case 'decimal' 			:	if((keyCode < 48 || keyCode > 57) && (keyCode != 45) && (keyCode != 46))			event.returnValue=false;	break;
		case 'decimal_positive'	:	if((keyCode < 48 || keyCode > 57) && (keyCode != 46))								event.returnValue=false;	break;
	}
}
//패턴체크후 틀리면 메시지 
function input_data_pattern_check(pattern_option,data,input_name,i){
	var pattern_existing ="";
	var pattern_no_using ="";
	var pattern_philsu="";
	var message="";
	switch (pattern_option) {
		case 'Num' 				: pattern_existing = /^[0-9\-]+$/;						pattern_philsu = /[{0-9}]/gi;						message="숫자,마이너스(-)만\n입력가능합니다.";							break;
		case 'NumPositive'		: pattern_existing = /^[0-9]+$/;						pattern_philsu = /[{0-9}]/gi;						message="숫자만\n입력가능합니다.";										break;
		case 'Decimal'			: pattern_existing = /^[0-9\.\-]+$/;					pattern_philsu = /[{0-9}]/gi;						message="숫자,마이너스(-),점(.)만\n 입력가능합니다.";						break;
		case 'DecimalPositive'	: pattern_existing = /^[0-9\.]+$/;						pattern_philsu = /[{0-9}]/gi;						message="숫자,점(.)만\n입력가능합니다.";								break;
		//case 'HanEngNumNomal'	: pattern_existing = /^[가-힣a-zA-Z0-9\(\)\[\]\~\!\@\#\$\^\*\_\-\.\<\>\/\=\,\|\%\s]+$/;	message="[?][&]['][\"][+] 을 제외한 문자만\n사용할 수 있습니다";		break;
		case 'HanEngNumNomal'	: pattern_no_using = /[\&\'\"\+\?]/gi;					pattern_philsu = /[{가-힣}{A-Z}{0-9}]/gi; 			message="반드시[한글 또는 영문 또는 숫자]가 포함되어야 하며\n[?][&]['][\"][+]을 제외한\n문자만 사용할 수 있습니다";		break;
		case 'HanEngNum'		: pattern_existing = /^[가-힣A-Z0-9\[\]\-\_\|\/\*\s]+$/gi;	pattern_philsu = /[{가-힣}{A-Z}{0-9}]/gi;			message="반드시[한글 또는 영문 또는 숫자]가 포함되어야 하며\n[대괄호][-][_][|][/][*]만\n사용할 수 있습니다";	break;
		case 'HanEng'			: pattern_existing = /^[가-힣A-Z\[\]\-\_\|\/\*\s]+$/gi;		pattern_philsu = /[{가-힣}{A-Z}]/gi;				message="반드시[한글 또는 영문]이 포함되어야 하며\n[대괄호][-][_][|][/][*]만\n사용할 수 있습니다";		break;
		case 'Han'				: pattern_existing = /^[가-힣\[\]\-\_\|\/\*\s]+$/gi;		pattern_philsu = /[{가-힣}]/gi;						message="반드시[한글]이 포함되어야 하며\n[대괄호][-][_][|][/][*]만\n사용할 수 있습니다";				break;
		case 'Eng'				: pattern_existing = /^[A-Z\[\]\-\_\|\/\*\s]+$/gi;			pattern_philsu = /[{A-Z}]/gi;						message="반드시[영문]이 포함되어야 하며\n[대괄호][-][_][|][/][*]만\n사용할 수 있습니다";				break;
		case 'HanNum'			: pattern_existing = /^[가-힣0-9\[\]\-\_\|\/\*\s]+$/gi;		pattern_philsu = /[{가-힣}{0-9}]/gi;				message="반드시[한글 또는 숫자]가 포함되어야 하며\n[대괄호][-][_][|][/][*]만\n사용할 수 있습니다";		break;
		case 'EngNum'			: pattern_existing = /^[A-Z0-9\[\]\-\_\|\/\*\s]+$/gi;		pattern_philsu = /[{A-Z}{0-9}]/gi;					message="반드시[영문 또는 숫자]가 포함되어야 하며\n[대괄호][-][_][|][/][*]만\n사용할 수 있습니다";		break;
	}
	//필수패터
	if(pattern_philsu && !pattern_philsu.test(data)){
		alert(message);	$("input[name='"+input_name+"']").eq(i).focus().select();	 return false;
	}
	//금지패턴
	if(pattern_no_using && data.match(pattern_no_using)){
		alert(message);	$("input[name='"+input_name+"']").eq(i).focus().select();	 return false;
	}
	//존재패턴
	if(pattern_existing && !pattern_existing.test(data)){
		alert(message);	$("input[name='"+input_name+"']").eq(i).focus().select();	 return false;
	}
}
//숫자타입 체크후 ture 혹은 false 리턴 주로 수량후 체크할때..
function common_number_check(number_type,n) {
	switch (number_type) {
		case 'Int' 		: return +n === n && !(n % 1);											break;	// -9007199254740990 to 9007199254740990 의 정수인지
		case 'Int8'		: return +n === n && !(n % 1) && n < 0x80 && n >= -0x80; 				break; //-128 to 127의 정수인지 
		case 'Int16'	: return +n === n && !(n % 1) && n < 0x8000 && n >= -0x8000;			break;//-32768 to 32767
		case 'Int32'	: return +n === n && !(n % 1) && n < 0x80000000 && n >= -0x80000000;	break;// -2147483648 to 2147483647
		case 'Uint'		: return +n === n && !(n % 1) && n >= 0;								break;//0 to 9007199254740990
		case 'Uint8'	: return +n === n && !(n % 1) && n < 0x100 && n >= 0;					break;//0 to 255
		case 'Uint16'	: return +n === n && !(n % 1) && n < 0x10000 && n >= 0;				break; //0 to 65535
		case 'Uint32'	: return +n === n && !(n % 1) && n < 0x100000000 && n >= 0;			break;// 0 to 4294967295

		case 'Float'	: return +n === n;												break;	//Any number including Infinity and -Infinity but not NaN
		case 'Float32'	: return +n === n && Math.abs(n) <= 3.4028234e+38;				break; //Any number from -3.4028234e+38 to 3.4028234e+38 (Single-precision floating-point format)
		case 'Float64'	: return +n === n && Math.abs(n) <= 1.7976931348623157e+308;	break;	//Any number excluding Infinity and -Infinity and NaN (Number.MAX_VALUE = 1.7976931348623157e+308)
	}
}
//체크박스체크
function checkbox_check_all(){
	var el=$("input:checkbox[name='my_check']");
	$(el).each(function(){
		if($(this).prop("checked") ) 	$(this).prop('checked', false) ;
		else							$(this).prop('checked', true) ;
		<!--테이블의 라인 색상 변경 tr의 클래스를 bg-gray 로 toggle한다.*/
		$(this).closest('tr').toggleClass("bg-gray", this.checked);									
	});
}
//체크박스에서 체크된 개수 알아내기
function get_count_checked_checkbox(){
	var chked=0; 
	var el=$("input:checkbox[name='my_check']");
	$(el).each(function(){
		if($(this).prop("checked") ) 	chked++;
	});
	return chked;
}
//체크박스에서 체크된 내용 알아내기
function get_cart_checked_checkbox(){
	var cart="";  
	var el=$("input:checkbox[name='my_check']");
	$(el).each(function(){
		if($(this).prop("checked"))			cart = cart + "," + $(this).val();
	});
	//제일 앞의 ,은 제거후 리턴한다.
	cart=cart.substring(1); //index의 1이후 자른다.
	return cart;
}
//URL로 데이터 넘길때 체크 왜냐면 IE에서 넘길수 있는 최대 길이는 2083자이므로 
function get_url_length(data){
	var data_length=parseInt(data.length,10);
	if(data_length >= 2000){
		alert('전송할 데이터 길이가 너무 큽니다.');
		return false;
	}else{
		return true;	
	}
}
/*
자동으로 바로 윗 라인의 데이터를 끌고와 완성
주로 스톤관리나 금및현금관리에서 많이 사용
*/
function common_auto_complete(element,i){
	if(i > 0){
		var cur_data=$(element).eq(i).val();    //현재 값
		var cur_tagName=$(element).eq(i).prop('tagName').toUpperCase(); //type의 종류
		var pre_data=$(element).eq(i-1).val(); //바로 위의 값..
		//console.log('pre_data:'+i+" :"+ pre_data);
		if((cur_data=='' || cur_data=='0'  ) && (pre_data!='')){ //현재 값이 없거나 값이 0일 때...
			if(cur_tagName=='INPUT')		$(element).eq(i).val(pre_data).focus().select();
			else							$(element).eq(i).val(pre_data).focus();
			return true;
		}else{
			return false;
		}
	}
}

//선택시..입력될 데이터..
function common_insert_data(insert_data){
	if(insert_data=='')	{
		alert('값이 존재하지 않습니다.');
		//window.close();
		return false;
	}
	var target_class_name=$("#target_class_name").val();	<!--클릭시 값이 들어갈 class의 name-->
	var target_class_index=$("#target_class_index").val(); <!--클릭시 값이 들어갈 class의 index 즉 위치-->
	if(target_class_index==null || target_class_index=='')	target_class_index=0;
	
	if(target_class_name=='')	{
		alert('선택한 값이 들어갈 타겟이 존재하지 않습니다.');
		window.close();
		return false;
	}
	window.opener.$("."+target_class_name).eq(target_class_index).val(insert_data).focus(); 
	
	//어디에서 불러왔는지에 따라서..다음을 실행한다.
	var from_where_window=$("#from_where_window").val(); 
	switch (from_where_window) {

		case 'account_buy_customer': //매입관리에서 매입처선택
			window.opener.loadData_customer_check();
			break;
		case 'tax_form': //세금계산서에서 사업자번호에 따를 거래처 정보가져오기
			window.opener.loadData_reg_no_customer();
			break;
		case 'sale_write_find_customer': //판매관리/대여관리에서 거래처 검색후 
			window.opener.loadData_company(); //거래처 정보가져오기
			break;
		case 'find_model_data': //주문관리 판매관리 재고관리 대여관리에서 모델 선택
			window.opener.get_local_model_data(target_class_index); //모델에 관련된 모든 사항 가져오기
			break;
	}
	window.close();	
}

<!-- 이미지 업로드-->
function common_preview_image(input) {
	var default_img=$('#default_img').val();  //기본 이미지
	var data=$(input).val().replace(/\s+/g,"");

	//새로운 이미지를 업로드하거나 바꾸려고할 때 아래 실행
	if(data == null || data == ''){
		$('#preView').attr('src', default_img);
	}else{
		var allowSize=$('#allowSize').val();
		var allowExt=$('#allowExt').val().toLowerCase();
		var message="";
		//확장자 확인
		var allowExt_split_arr=allowExt.split(","); //split함수를 사용하여 ,되어 있는 것을 분리하여 배열로 저장
		
		var ext = data.split('.').pop().toLowerCase();
		if($.inArray(ext, allowExt_split_arr) == -1) {
			message='현재 파일 확장자는 ['+ext+'] 입니다.';
			message+='\n['+allowExt+']를 가진 확장자 파일만 업로드 할수 있습니다.';
			alert(message);
			$('#file').replaceWith($('#file').clone(true));
			$('#file').val('').focus(); 
			$('#preView').attr('src', default_img);  
			return false;
		}

		//FileReader가 있는 브라우져 있냐는 따라...
		if(window.FileReader ) { 	  /*IE 9 이상에서는 FileReader  이용*/
			 var cur_file_size = Math.round($("#file")[0].files[0].size / 1024);
			 if(cur_file_size > allowSize){
				message='현재 파일 용량 ['+cur_file_size+']KB 입니다.';
				message+='\n최대 용량['+allowSize+']KB 이하로 조정하기 바랍니다. ';
				alert(message);
				$('#file').replaceWith($('#file').clone(true));
				$('#file').val('').focus(); 
				$('#preView').attr('src', default_img); 
				return false;
			 }else{
				 var reader = new FileReader();
				 reader.onload = function (e) {
					$('#preView').attr('src', e.target.result);
					$('#virtual_img').attr('src', e.target.result);	//가상 이미지 경로
					re_attach_image($("#virtual_img"));	//이미지 사이즈 조절을 위해 객체를 함수로 보낸다
				};
				reader.readAsDataURL(input.files[0]);
				return input.files[0].name;  // 파일명 return
			}																
		}else {
			//jquery.form.js를 통한  파일업로두 후 파일사이즈,확장자 체크
			//참조사이트 http://www.9lessons.info/2011/08/ajax-image-upload-without-refreshing.html
			var upload_path_root=$('#upload_path_root').val();  //기본 이미지
			var upload_path_folder_sub=$('#upload_path_folder_sub').val();  //업로드 폴더의 하부
			
			var upload_path_folder="/temp_file/"+upload_path_folder_sub;   //업로드 폴더
			$("form").ajaxForm({ //form의 ID 혹은 form를 적어둔다.
				type:'POST',
				data : { 
					upload_path_root  	: upload_path_root,		//root 경로 /home/계정/html/
					upload_path_folder	: upload_path_folder,	//임시 업로드 폴더
					allowSize				: allowSize, 				//최대 파일 용량 KB
					allowExt					: allowExt 					//파일 확장자 제한
				},
				url:'/zconfig/common_php/loadData_file_upload.php', //업로드체크 및 업로드
				async: false,
				dataType:'script',
				success:function(data,script){
					var return_src=$('#temp_file').val();
					if(return_src){
						$('#preView').attr('src',upload_path_folder+return_src);		//root경로+업로드된 이미지 파일
						$('#virtual_img').attr('src',upload_path_folder+return_src);	//가상 이미지 경로
						re_attach_image($("#virtual_img"));	//이미지 사이즈 조절을 위해 객체를 함수로 보낸다
					}else{
						$('#preView').attr('src', default_img);
					}
				}
			}).submit();
		} //if filereader
	}

	//리사이징하기 위한 이미지 객체를 함수로 받는다.
	function re_attach_image(img){
		img.load(function () {		//이미지가 로딩이 완료 된 후
			var w	= img.width();	//원본 이미지 가로길이
			var h	= img.height();	//원본 이미지 세로길이
																						
			if(w<150 && h<150)		$('#preView').css({'width':'auto','height':'auto'});		//가로, 세로가 150px보다 작은 이미지(원본 크기로 출력)
			else if(w/h > 1.3)		$('#preView').css({'width':'100%','height':'auto'});	//가로, 세로의 비율이 1.3 보다 클 때 width는 100%
			else					$('#preView').css({'width':'auto','height':'100%'});	//그 외 나머지 모든 경우 높이를 150px 틀에 자동으로 맞춘다.
		});
	}
}
<!-- 파일 업로드-->	
function common_file_check(input) {
	var data=$(input).val()
	if(data != null || data != ''){
		var allowSize=$('#allowSize').val();
		var allowExt=$('#allowExt').val().toLowerCase();
		var message="";
		//확장자 점검
		var allowExt_split_arr=allowExt.split(","); //split함수를 사용하여 ,되어 있는 것을 분리
		var ext = data.split('.').pop().toLowerCase();
		if($.inArray(ext, allowExt_split_arr) == -1) { /*inArray()를 이용해 배열 내의 값을 찾아서 인덱스를 반환합니다.(요소가 없을 경우 -1을 반환).*/
			message='현재 파일 확장자는 ['+ext+'] 입니다.';
			message+='\n['+allowExt+']를 가진 확장자 파일만 업로드 할수 있습니다.';
			alert(message);
			$('#file').replaceWith($('#file').clone(true));
			$('#file').val('').focus(); 
			return false;
		}

		//FileReader가 있는 브라우져 있냐는 따라...
		if(window.FileReader ) { 	  /*IE 9 이상에서는 FileReader  이용*/
			 var cur_file_size = Math.round($("#file")[0].files[0].size / 1024);
			 if(cur_file_size > allowSize){
				message='현재 파일 용량 ['+cur_file_size+']KB 입니다.';
				message+='\n최대 용량['+allowSize+']KB 이하로 조정하기 바랍니다. ';
				alert(message);
				$('#file').replaceWith($('#file').clone(true));
				$('#file').val('').focus(); 
				return false;
			 }else{
				 var reader = new FileReader();
				// reader.onload = function(e){};
				 reader.readAsDataURL(input.files[0]);
				 return input.files[0].name;  // 파일명 return
			}																
		}else {
			//jquery.form.js를 통한  파일업로두 후 파일사이즈,확장자 체크
			//참조사이트 http://www.9lessons.info/2011/08/ajax-image-upload-without-refreshing.html
			var upload_path_root=$('#upload_path_root').val();  //루트폴더
			         
			var upload_path_folder_sub=$('#upload_path_folder_sub').val();  //업로드 폴더의 하부
			var upload_path_folder="/temp_file/"+upload_path_folder_sub;   //업로드 폴더
			
			  
			$("form").ajaxForm({ //form의 ID 혹은 form를 적어둔다.
				type:'POST',
				data : { 
					upload_path_root  	: upload_path_root,		//root 경로 /home/계정/html/
					upload_path_folder	: upload_path_folder,	//임시 업로드 폴더
					allowSize			: allowSize, 				//최대 파일 용량 KB
					allowExt			: allowExt 					//파일 확장자 제한
				},
				url:'/zconfig/common_php/loadData_file_upload.php', //업로드체크 및 업로드
				dataType:'script',
				success:function(data){}
			}).submit();
		} //if filereader
	}
}
/*요일구하기*/
function common_get_yoil(sDate) {
 
    var yy = parseInt(sDate.substr(0, 4), 10);
    var mm = parseInt(sDate.substr(5, 2), 10);
    var dd = parseInt(sDate.substr(8), 10);
    var d = new Date(yy,mm - 1, dd);
    /*
	var weekday=new Array(7);
     weekday[0]="일";
     weekday[1]="월";
     weekday[2]="화";
     weekday[3]="수";
     weekday[4]="목";
     weekday[5]="금";
     weekday[6]="토";
	*/
	 var weekday = new Array('일', '월', '화', '수', '목', '금', '토');
 
    return weekday[d.getDay()];
 }

