/*! jQuery UI - v1.10.4 - 2014-01-26
* http://jqueryui.com
* Copyright 2014 jQuery Foundation and other contributors; Licensed MIT */

jQuery(function(t){t.datepicker.regional.ko={closeText:"닫기",prevText:"이전달",nextText:"다음달",currentText:"오늘",monthNames:["1월","2월","3월","4월","5월","6월","7월","8월","9월","10월","11월","12월"],monthNamesShort:["1월","2월","3월","4월","5월","6월","7월","8월","9월","10월","11월","12월"],dayNames:["일요일","월요일","화요일","수요일","목요일","금요일","토요일"],dayNamesShort:["일","월","화","수","목","금","토"],dayNamesMin:["일","월","화","수","목","금","토"],weekHeader:"주",dateFormat:"yy-mm-dd",firstDay:0,isRTL:!1,showMonthAfterYear:!0,yearSuffix:""},t.datepicker.setDefaults(t.datepicker.regional.ko)});

$(function() {
	var cur_date=$('#Common_ToDay').val();
	var min_day=$('#min_day').val();
	var max_day=$('#max_day').val();

	$('.calendar').datepicker({
		//showWeek:true,//주표시
		showOtherMonths: true, //다른달 표시     
		selectOtherMonths: true, //다른달 선택
		showButtonPanel: true, 	//하단에 금일과 닫기 버튼 표시
		yearRange: "c-10:c+5",  //표시 년도 표시
		changeMonth: true,  //월을 변경할수 있는 select박스 표시
		changeYear: true   //년을 변경할수 있는 select박스 표시
		
	});
	//최소 오늘 부터 나오게..주문예정일 혹은 앞으로의 일정
	$('.calendar_min').datepicker({
		minDate: cur_date, 
		
		showOtherMonths: true,     
		selectOtherMonths: true, 
		showButtonPanel: true, 	
		yearRange: "c-5:c+10",
		changeMonth: true,
		changeYear: true
	});
	//최대 오늘 부터 나오게  주용도:생일날
	$('.calendar_max').datepicker({
		maxDate: cur_date,
		showOtherMonths: true,     
		selectOtherMonths: true, 
		showButtonPanel: true, 	
		yearRange: "c-20:c+5",
		changeMonth: true,
		changeYear: true
	});

	//날짜의 범위를 설정한다.
	$('.calendar_range').datepicker({
		minDate: min_day,
		maxDate: max_day,
		
		showOtherMonths: true,     
		selectOtherMonths: true, 
		showButtonPanel: true, 	
		yearRange: "c-10:c+5",
		changeMonth: true,
		changeYear: true
	});
	//오늘 날짜만 입력가능하게 한다.
	$('.calendar_today').datepicker({
		minDate: cur_date,
		maxDate: cur_date,
		
		showOtherMonths: true,     
		selectOtherMonths: true, 
		showButtonPanel: true, 	
		yearRange: "c-10:c+5",
		changeMonth: true,
		changeYear: true
	});
/*************아이콘 표시..*********************/
	$('.calendar_icon').datepicker({
		showOn: "button",
		buttonImage: "/zconfig/jquery/plugin/datepicker/calendar.gif",
		//buttonText: "<i class='fa fa-calendar'></i>",
		buttonText: "클릭",
		buttonImageOnly: true, 
		
		//showWeek:true,
		showOtherMonths: true, 
		selectOtherMonths: true, 
		showButtonPanel: true, 
		yearRange: "c-10:c+5",
		changeMonth: true,  
		changeYear: true,
		onClose: function() {
			this.focus();
      	} 
	});

	//i최소 오늘 부터 나오게..주로 생일날 쓰임
	$('.calendar_min_icon').datepicker({
		minDate: cur_date,
		showOn: "button",
		buttonImage: "/zconfig/jquery/plugin/datepicker/calendar.gif",
		buttonText: "클릭",
		buttonImageOnly: true, 
		
		showOtherMonths: true,     
		selectOtherMonths: true, 
		showButtonPanel: true, 	
		yearRange: "c-5:c+20",
		changeMonth: true,
		changeYear: true,
		onClose: function() {        	
			this.focus();
      	}

	});
	//최대 오늘 부터 나오게 용도
	$('.calendar_max_icon').datepicker({
		maxDate: cur_date,

		showOn: "button",
		buttonImage: "/zconfig/jquery/plugin/datepicker/calendar.gif",
		buttonText: "클릭",
		buttonImageOnly: true, 

		showOtherMonths: true,     
		selectOtherMonths: true, 
		showButtonPanel: true, 	
		yearRange: "c-20:c+5",
		changeMonth: true,
		changeYear: true,
		onClose: function() {
			this.focus();
      	}
	});


	//날짜의 범위를 설정한다.
	$('.calendar_range_icon').datepicker({
		minDate: min_day,
		maxDate: max_day,
		showOn: "button",
		buttonImage: "/zconfig/jquery/plugin/datepicker/calendar.gif",
		buttonText: "클릭",
		buttonImageOnly: true, 
		
		showOtherMonths: true,     
		selectOtherMonths: true, 
		showButtonPanel: true, 	
		yearRange: "c-10:c+5",
		changeMonth: true,
		changeYear: true,
		onClose: function() {
        	this.focus();
      	} 
	});
	//당일 날짜만  입력가능하게한다 설정한다.
	$('.calendar_today_icon').datepicker({
		minDate: cur_date,
		maxDate: cur_date,
		showOn: "button",
		buttonImage: "/zconfig/jquery/plugin/datepicker/calendar.gif",
		buttonText: "클릭",
		buttonImageOnly: true, 
		
		showOtherMonths: true,     
		selectOtherMonths: true, 
		showButtonPanel: true, 	
		yearRange: "c-10:c+5",
		changeMonth: true,
		changeYear: true,
		onClose: function() {
        	this.focus();
      	} 		
	});
});

