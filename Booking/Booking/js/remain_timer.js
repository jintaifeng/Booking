$(function(){
	
	var login_session_id=$("#login_session_id").val(); //세션값
	if(!login_session_id){
		alert('세션값이 없어 로그 아웃되었습니다.');
		window.location.href='/';
	}
	//var popup_window_check="N"; //팝업창 체크
	//if (window.opener && window.opener !== window) 	popup_window_check="Y";
	
	//1초 단위로 인터벌을 생성하여 ajax를 이용한 로그아웃 남은 시간  표시
	var remain_timer=setInterval(function () {
		$.ajax({
			type:'POST',
			url:'/zconfig/remain_logout_alert/loadData_remain_timer.php',
			data : { 
				login_session_id:login_session_id
			},
			dataType:'script',
			success:function(data,script){
				//eval(script);
			}
		});
		
		var real_remain_second=parseInt($("#logout_real_remain_second").val()); //남은시간
		if(real_remain_second != null && real_remain_second != '' ){
			//2분이 남으면면..연창 요청 팝업창 열기
			//if(real_remain_second > 0 && real_remain_second < (2*60)){
				//window.open('/zconfig/remain_logout_alert/index.htm?real_remain_second='+real_remain_second,'real_remain_second_popup','toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,resizable=no,width=400,height=225');
			
			//}else 
			if(real_remain_second <= 10){
				 clearInterval(remain_timer); /* 인터벌을 종료 즉 타이머 종료 */
				 if(window.opener && window.opener != window){
					 window.close();
				}else{
					// alert('로그 아웃되었습니다.');
					// window.location.href='/zconfig/logout.php';
				}
				window.location.href='/zconfig/logout.php';
			}
		}
	}, 10000); /* 1초 1000 2초단위 인터벌 */	
});
