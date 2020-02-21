var addEvent = function addEvent(element, eventName, func) {
	if (element.addEventListener) {
    	return element.addEventListener(eventName, func, false);
    } else if (element.attachEvent) {
        return element.attachEvent("on" + eventName, func);
    }
};

addEvent(document.getElementById('open-left'), 'click', function(){
	if( snapper.state().state=="left" ){
        snapper.close();
    } else {
        snapper.open('left');
    }
});

snapper.on('open', function(){
  	var bC = document.querySelector("#buttonCover");
  	bC.classList.add("coverPane");
});

snapper.on('close', function(){
  	var bC = document.querySelector("#buttonCover");
  	bC.classList.remove("coverPane");
});

snapper.on('animated', function(){
	var bC = document.querySelector("#buttonCover");
	if ( snapper.state().state=="left" ) {
  		bC.classList.add("coverPane");
	} else if (snapper.state().state=="closed") {
		bC.classList.remove("coverPane");
	}	
});

snapper.on('drag', function(){
	/*var div = document.getElementById('textDiv');
    div.textContent = snapper.state().state + " " + snapper.state().info.towards + " " + dragCounter + " " + snapper.state().info.translation.percentage.toFixed(1);
*/	
  	if (snapper.state().state=="closed" && snapper.state().info.translation.percentage > 0) {
  		var bC = document.querySelector("#buttonCover");
		bC.classList.add("coverPane");
	}
	
});

snapper.on('end', function(){
  	dragCounter = 0;		
});

/* Prevent Safari opening links when viewing as a Mobile App */
(function (a, b, c) {
    if(c in b && b[c]) {
        var d, e = a.location,
            f = /^(a|html)$/i;
        a.addEventListener("click", function (a) {
            d = a.target;
            while(!f.test(d.nodeName)) d = d.parentNode;
            "href" in d && (d.href.indexOf("http") || ~d.href.indexOf(e.host)) && (a.preventDefault(), e.href = d.href)
        }, !1)
    }
})(document, window.navigator, "standalone");