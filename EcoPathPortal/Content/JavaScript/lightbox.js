var musicPlayer = ""; 
var fileLoadingImage = "/EcoPathPortal/Content/loading.gif"; 
var fileBottomNavCloseImage1 = "/EcoPathPortal/Content/close1.gif"; 
var fileBottomNavCloseImage2 = "/EcoPathPortal/Content/close2.gif"; 
var resizeSpeed = 8; 
var borderSize = 10; 
var slideShowWidth = 250; 
var slideShowHeight = 150; 
var SlideShowStartImage = "/EcoPathPortal/Content/start.gif"; 
var SlideShowStopImage = "/EcoPathPortal/Content/stop.gif"; 
var slideshow = 0; 
var foreverLoop = 1; 
var loopInterval = 3500; 
var resize = 0; 
var imageArray = new Array; 
var activeImage; 

if (resizeSpeed > 10) { resizeSpeed = 10; }
if (resizeSpeed < 1) { resizeSpeed = 1; }
resizeDuration = (11 - resizeSpeed) * 0.15; 

var so = null; 
var objSlideShowImage; 
var objLightboxImage; 
var objImageDataContainer; 
var keyPressed = false; 
var slideshowMusic = null; 
var firstTime = 1; 
var saveSlideshow; 
var saveForeverLoop; 
var saveLoopInterval; 
var saveSlideShowWidth; 
var saveSlideShowHeight; 

Object.extend(Element, { 
	getWidth: function(element) { 
		element = $(element); 
		return element.offsetWidth;
	}, 
	setWidth: function(element,w) { element = $(element); 
	element.style.width = w +"px";}, setHeight: function(element,h) { element = $(element); 
	element.style.height = h +"px";}, setTop: function(element,t) { element = $(element); 
	element.style.top = t +"px";}, setSrc: function(element,src) { element = $(element); 
	element.src = src;}, setHref: function(element,href) { element = $(element); 
	element.href = href;}, setInnerHTML: function(element,content) { element = $(element); 
	element.innerHTML = content;}
}); 

Array.prototype.removeDuplicates = function () { 
	for(i = 1; i < this.length; i++) { 
		if(this[i][0] == this[i-1][0]) { this.splice(i,1); }
	}
}; 

Array.prototype.empty = function () { 
	for(i = 0; i <= this.length; i++) { this.shift(); }
}; 

var Lightbox = Class.create(); 

Lightbox.prototype = { 
	initialize: function() { 
		if (!document.getElementsByTagName){ return; }
		var anchors = document.getElementsByTagName('a'); 
		for (var i=0; i<anchors.length; i++) { 
			var anchor = anchors[i]; 
			var relAttribute = String(anchor.getAttribute('rel')); 
			if (anchor.getAttribute('href') && (relAttribute.toLowerCase().match('lightbox'))) { 
				anchor.onclick = function () {
					myLightbox.start(this); 
					return false;
				};
			}
		}
		var objBody = document.getElementsByTagName("body").item(0); 
		var objOverlay = document.createElement("div"); 
		objOverlay.setAttribute('id','overlay'); 
		objOverlay.style.display = 'none'; 
		objOverlay.onclick = function() { 
			myLightbox.end(); 
			return false;
		}; 
		objBody.appendChild(objOverlay); 
		var objLightbox = document.createElement("div"); 
		objLightbox.setAttribute('id','lightbox'); 
		objLightbox.style.display = 'none'; 
		objBody.appendChild(objLightbox); 
		var objOuterImageContainer = document.createElement("div"); 
		objOuterImageContainer.setAttribute('id','outerImageContainer'); 
		objLightbox.appendChild(objOuterImageContainer); 
		var objImageContainer = document.createElement("div"); 
		objImageContainer.setAttribute('id','imageContainer'); 
		objOuterImageContainer.appendChild(objImageContainer); 
		objLightboxImage = document.createElement("img"); 
		objLightboxImage.setAttribute('id','lightboxImage'); 
		objLightboxImage.setAttribute('width',''); 
		objLightboxImage.setAttribute('height',''); 
		objLightboxImage.setAttribute('galleryimg','no'); 
		objLightboxImage.setAttribute('oncontextmenu','return false;'); 
		objLightboxImage.setAttribute('onmousedown','return false;'); 
		objLightboxImage.setAttribute('onmousemove','return false;'); 
		objImageContainer.appendChild(objLightboxImage); 
		var objHoverNav = document.createElement("div"); 
		objHoverNav.setAttribute('id','hoverNav'); 
		objImageContainer.appendChild(objHoverNav); 
		var objPrevLink = document.createElement("a"); 
		objPrevLink.setAttribute('id','prevLink'); 
		objPrevLink.setAttribute('href','#'); 
		objPrevLink.setAttribute('onFocus', 'if (this.blur) this.blur()'); 
		objHoverNav.appendChild(objPrevLink); 
		var objNextLink = document.createElement("a"); 
		objNextLink.setAttribute('id','nextLink'); 
		objNextLink.setAttribute('href','#'); 
		objNextLink.setAttribute('onFocus', 'if (this.blur) this.blur()'); 
		objHoverNav.appendChild(objNextLink); 
		var objLoading = document.createElement("div"); 
		objLoading.setAttribute('id','loading'); 
		objImageContainer.appendChild(objLoading); 
		var objLoadingLink = document.createElement("a"); 
		objLoadingLink.setAttribute('id','loadingLink'); 
		objLoadingLink.setAttribute('href','#'); 
		objLoadingLink.setAttribute('onFocus', 'if (this.blur) this.blur()'); 
		objLoadingLink.onclick = function() { 
			myLightbox.end(); 
			return false;
		}; 
		objLoading.appendChild(objLoadingLink); 
		var objLoadingImage = document.createElement("img"); 
		objLoadingImage.setAttribute('src', fileLoadingImage); 
		objLoadingLink.appendChild(objLoadingImage); 
		objImageDataContainer = document.createElement("div"); 
		objImageDataContainer.setAttribute('id','imageDataContainer'); 
		objImageDataContainer.className = 'clearfix'; 
		objLightbox.appendChild(objImageDataContainer); 
		var objImageData = document.createElement("div"); 
		objImageData.setAttribute('id','imageData'); 
		objImageDataContainer.appendChild(objImageData); 
		var objImageDetails = document.createElement("div"); 
		objImageDetails.setAttribute('id','imageDetails'); 
		objImageData.appendChild(objImageDetails); 
		var objCaption = document.createElement("span"); 
		objCaption.setAttribute('id','caption'); 
		objCaption.setAttribute('align','left'); 
		objImageDetails.appendChild(objCaption); 
		var objNumberDisplay = document.createElement("span"); 
		objNumberDisplay.setAttribute('align','left'); 
		objNumberDisplay.setAttribute('id','numberDisplay'); 
		objImageDetails.appendChild(objNumberDisplay); 
		var objBottomNav = document.createElement("div"); 
		objBottomNav.setAttribute('id','bottomNav'); 
		objImageData.appendChild(objBottomNav); 
		var objBottomNavCloseLink = document.createElement("a"); 
		objBottomNavCloseLink.setAttribute('id','bottomNavClose'); 
		objBottomNavCloseLink.setAttribute('href','#'); 
		objBottomNavCloseLink.setAttribute('onFocus', 'if (this.blur) this.blur()'); 
		objBottomNavCloseLink.onclick = function() { 
			myLightbox.end(); 
			window.clearInterval(chid); 
			history.go(-1); 
			return false;
		}; 
		objBottomNavCloseLink.onmouseover = function () { objBottomNavCloseImage.setAttribute('src', fileBottomNavCloseImage2); }; 
		objBottomNavCloseLink.onmouseout = function () { objBottomNavCloseImage.setAttribute('src', fileBottomNavCloseImage1); }; 
		objBottomNav.appendChild(objBottomNavCloseLink); 
		var objBottomNavCloseImage = document.createElement("img"); 
		objBottomNavCloseImage.setAttribute('src', fileBottomNavCloseImage1); 
		objBottomNavCloseImage.setAttribute('align', 'right'); 
		objBottomNavCloseImage.setAttribute('name', 'close'); 
		objBottomNavCloseImage.setAttribute('height', '11'); 
		objBottomNavCloseImage.setAttribute('width', '48'); 
		objBottomNavCloseLink.appendChild(objBottomNavCloseImage); 
		var objSlideShowLink = document.createElement("a"); 
		objSlideShowLink.setAttribute('id','slideshowLink'); 
		objSlideShowLink.setAttribute('href','#'); 
		objSlideShowLink.setAttribute('title','Slideshow'); 
		objSlideShowLink.setAttribute('onFocus', 'if (this.blur) this.blur()'); 
		objSlideShowLink.onclick = function() { 
			myLightbox.toggleSlideShow(); 
			return false;
		}; 
		objBottomNav.appendChild(objSlideShowLink); 
		objSlideShowImage = document.createElement("img"); 
		objSlideShowImage.setAttribute('src', SlideShowStartImage); 
		objSlideShowImage.setAttribute('height', '14'); 
		objSlideShowImage.setAttribute('width', '53'); 
		objSlideShowLink.appendChild(objSlideShowImage); 
		var objFlashPlayer = document.createElement("div"); 
		objFlashPlayer.setAttribute('id','flashPlayer'); 
		objBottomNav.appendChild(objFlashPlayer);
	}, 
	start: function(imageLink) { 
		firstTime = 1; 
		saveSlideshow = slideshow; 
		saveForeverLoop = foreverLoop; 
		saveLoopInterval = loopInterval; 
		saveSlideShowWidth = slideShowWidth; 
		saveSlideShowHeight = slideShowHeight; 
		hideSelectBoxes(); 
		var arrayPageSize = getPageSize(); 
		Element.setHeight('overlay', arrayPageSize[1]); 
		Effect.Appear('overlay', { duration: 0.2, from: 0.0, to: 0.8 }); 
		imageArray = []; 
		imageNum = 0; 
		if (!document.getElementsByTagName){ return; }
		var anchors = document.getElementsByTagName('a'); 
		if((imageLink.getAttribute('rel') == 'lightbox')) { imageArray.push(new Array(imageLink.getAttribute('href'), imageLink.getAttribute('caption'))); } 
		else { 
			for (var i=0; i<anchors.length; i++) { 
				var anchor = anchors[i]; 
				if (anchor.getAttribute('href') && (anchor.getAttribute('rel') == imageLink.getAttribute('rel'))) { 
					imageArray.push(new Array(anchor.getAttribute('href'), anchor.getAttribute('caption'))); 
					if (imageArray.length == 1) { 
						slideshowMusic = anchor.getAttribute('music'); 
						if (slideshowMusic == null) { Element.hide('flashPlayer'); } 
						else { Element.show('flashPlayer'); }
						var startSlideshow = anchor.getAttribute('startslideshow'); 
						if (startSlideshow != null) { if (startSlideshow == "false") slideshow = 0; }
						var forever = anchor.getAttribute('forever'); 
						if (forever != null) { 
							if (forever == "true") foreverLoop = 1; 
							else foreverLoop = 0;
						}
						var slideDuration = anchor.getAttribute('slideDuration'); 
						if (slideDuration != null) { loopInterval = slideDuration * 1000; }
						var width = anchor.getAttribute('slideshowwidth'); 
						if (width != null) { slideShowWidth = width *1; }
						var height = anchor.getAttribute('slideshowheight'); 
						if (height != null) { slideShowHeight = height *1; }
					}
				}
			}
			imageArray.removeDuplicates(); 
			while(imageArray[imageNum][0] != imageLink.getAttribute('href')) { imageNum++; }
		}
		this.changeImageByTimer(imageNum);
	}, 
	showLightBox: function() { 
		var arrayPageSize = getPageSize(); 
		var arrayPageScroll = getPageScroll(); 
		var lightboxTop = arrayPageScroll[1] + (arrayPageSize[3] / 15); 
		Element.setTop('lightbox', lightboxTop); 
		Element.show('lightbox');
	}, 
	changeImageByTimer: function(imageNum) { 
		activeImage = imageNum; 
		this.imageTimer = setTimeout(function() { 
			this.showLightBox(); 
			this.changeImage(activeImage);
		}.bind(this), 10);
	}, 
	changeImage: function(imageNum) { 
		activeImage = imageNum; 
		Element.show('loading'); 
		Element.hide('lightboxImage'); 
		Element.hide('hoverNav'); 
		Element.hide('prevLink'); 
		Element.hide('nextLink'); 
		Element.hide('imageDataContainer'); 
		Element.hide('numberDisplay'); 
		Element.hide('slideshowLink'); 
		imgPreloader = new Image(); 
		imgPreloader.onload = function() { 
			Element.setSrc('lightboxImage', imageArray[activeImage][0]); 
			objLightboxImage.setAttribute('width', imgPreloader.width); 
			objLightboxImage.setAttribute('height', imgPreloader.height); 
			if ((imageArray.length > 1) && (slideShowWidth != -1 || slideShowHeight != -1)) { 
				if ( (slideShowWidth >= imgPreloader.width) && (slideShowHeight >= imgPreloader.height) ) { myLightbox.resizeImageContainer(slideShowWidth, slideShowHeight); } 
				else { myLightbox.resizeImageAndContainer(imgPreloader.width, imgPreloader.height); }
			} else { myLightbox.resizeImageAndContainer(imgPreloader.width, imgPreloader.height);}
		}
		imgPreloader.src = imageArray[activeImage][0];
	}, 
	resizeImageAndContainer: function(imgWidth, imgHeight) { 
		if(resize == 1) { 
			useableWidth = 0.9; 
			useableHeight = 0.8; 
			var arrayPageSize = getPageSize(); 
			windowWidth = arrayPageSize[2]; 
			windowHeight = arrayPageSize[3]; 
			scaleX = 1; 
			scaleY = 1; 
			if ( imgWidth > windowWidth * useableWidth ) scaleX = (windowWidth * useableWidth) / imgWidth; 
			if ( imgHeight > windowHeight * useableHeight ) scaleY = (windowHeight * useableHeight) / imgHeight; 
			scale = Math.min( scaleX, scaleY ); 
			imgWidth *= scale; 
			imgHeight *= scale; 
			objLightboxImage.setAttribute('width', imgWidth); 
			objLightboxImage.setAttribute('height', imgHeight);
		}
		this.resizeImageContainer((imgWidth < 320) ? 320 : imgWidth, imgHeight);
	}, 
	resizeImageContainer: function (imgWidth, imgHeight) { 
		this.wCur = Element.getWidth('outerImageContainer'); 
		this.hCur = Element.getHeight('outerImageContainer'); 
		this.xScale = ((imgWidth + (borderSize * 2)) / this.wCur) * 100; 
		this.yScale = ((imgHeight + (borderSize * 2) + 20) / this.hCur) * 100; 
		wDiff = (this.wCur - borderSize * 2) - imgWidth; 
		hDiff = (this.hCur - borderSize * 2) - imgHeight; 
		if (!(hDiff == 0)) { new Effect.Scale('outerImageContainer', this.yScale, {scaleX: false, duration: resizeDuration, queue: 'front'}); }
		if (!(wDiff == 0)) { new Effect.Scale('outerImageContainer', this.xScale, {scaleY: false, delay: resizeDuration, duration: resizeDuration}); }
		if((hDiff == 0) && (wDiff == 0)){ 
			if (navigator.appVersion.indexOf("MSIE")!=-1) { pause(250); } 
			else { pause(100); }
		}
		Element.setHeight('imageContainer', imgHeight);
		Element.setHeight('prevLink', imgHeight); 
		Element.setHeight('nextLink', imgHeight); 
		Element.setWidth( 'imageDataContainer', ((imgWidth < 320) ? 320 : imgWidth) + (borderSize * 2)); 
		this.showImage();
	}, 
	showImage: function() { 
		Element.hide('loading'); 
		new Effect.Appear('lightboxImage', { duration: 0.5, queue: 'end', afterFinish: function(){ myLightbox.updateDetails();} }); 
		this.preloadNeighborImages();
	}, 
	updateDetails: function() { 
		Element.show('caption'); 
		if (imageArray[activeImage][1] != '') { Element.setInnerHTML( 'caption', imageArray[activeImage][1]); } 
		else { Element.setInnerHTML( 'caption', "&nbsp;"); }
		if(imageArray.length>1) { 
			var ndhtml; 
			Element.show('numberDisplay'); 
			ndhtml=""+eval(activeImage+1)+" of "+imageArray.length; 
			if(eval(activeImage+1)>1){ ndhtml="<a id='prevLink2'>Previous</a>&nbsp;&nbsp;-&nbsp;&nbsp;"+ndhtml; }
			if(eval(activeImage+1)<imageArray.length){ ndhtml=ndhtml+"&nbsp;&nbsp;-&nbsp;&nbsp;<a id='nextLink2'>Next</a>"; }
			Element.setInnerHTML('numberDisplay',ndhtml);
		}
		if (1 == 1) { 
			new Effect.Parallel( 
				[new Effect.SlideDown( 'imageDataContainer', { sync: true, duration: resizeDuration + 0.25, from: 0.0, to: 1.0 } ), new Effect.Appear('imageDataContainer', { sync: true, duration: 1.0 })], 
				{ duration: 0.65, afterFinish: function() { myLightbox.updateNav();} } 
			);
		} 
		else { myLightbox.updateNav(); }
		if (imageArray.length > 1) { 
			Element.show('flashPlayer'); 
			Element.show('slideshowLink'); 
		}
		else { 
			Element.hide('flashPlayer'); 
			Element.hide('slideshowLink');
		}
		if (slideshow == 1) { this.startSlideShow(); }
	}, 
	updateNav: function() { 
		Element.show('hoverNav'); 
		if(activeImage != 0) { 
			Element.show('prevLink'); 
			document.getElementById('prevLink').onclick = function() { 
				if (slideshow == 1) keyPressed = true; 
				myLightbox.changeImage(activeImage - 1); 
				return false;
			}
			document.getElementById('prevLink2').onclick = function() { 
				if (slideshow == 1) keyPressed = true; 
				myLightbox.changeImage(activeImage - 1); 
				return false;
			}
		}
		if(activeImage != (imageArray.length - 1)) { 
			Element.show('nextLink'); 
			document.getElementById('nextLink').onclick = function() { 
				if (slideshow == 1) keyPressed = true; 
				myLightbox.changeImage(activeImage + 1); 
				return false;
			}
			document.getElementById('nextLink2').onclick = function() { 
				if (slideshow == 1) keyPressed = true; 
				myLightbox.changeImage(activeImage+1);
				return false;
			}
		}
		this.enableKeyboardNav(); 
		if (firstTime == 1) { 
			firstTime = 0; 
			if (imageArray.length > 1 && slideshow == 1) this.showMusicPlayer(); 
			if (slideshow == 1) this.playMusic();
		}
	}, 
	enableKeyboardNav: function() { 
		document.onkeydown = this.keyboardAction;
	}, 
	disableKeyboardNav: function() { 
		document.onkeydown = '';
	}, 
	keyboardAction: function(e) { 
		if (e == null) { keycode = event.keyCode; } 
		else { keycode = e.which; }
		key = String.fromCharCode(keycode).toLowerCase(); 
		if((key == 'x') || (key == 'o') || (key == 'c')) { myLightbox.end(); } 
		else if((keycode == 188) || (key == 'p') || (keycode == 37)) { 
			if(activeImage != 0) { 
				if (slideshow == 1) keyPressed = true; 
				myLightbox.disableKeyboardNav(); 
				myLightbox.changeImage(activeImage - 1);
			}
		} 
		else if((keycode == 190) || (key == 'n') || (keycode == 39)) { 
			if(activeImage != (imageArray.length - 1)) { 
				if (slideshow == 1) keyPressed = true; 
				myLightbox.disableKeyboardNav(); 
				myLightbox.changeImage(activeImage + 1);
			}
		} 
		else if(key == 's') { myLightbox.toggleSlideShow(); }
	}, 
	preloadNeighborImages: function() { 
		if((imageArray.length - 1) > activeImage) { 
			preloadNextImage = new Image(); 
			preloadNextImage.src = imageArray[activeImage + 1][0];
		}
		if(activeImage > 0) { 
			preloadPrevImage = new Image(); 
			preloadPrevImage.src = imageArray[activeImage - 1][0];
		}
	}, 
	toggleSlideShow: function() { 
		if(slideshow == 1) this.stopSlideShow(); 
		else { 
			if(activeImage == (imageArray.length-1)) { 
				slideshow = 1; 
				this.changeImage(0);
			} 
			else { this.startSlideShow(); }
		}
	}, 
	startSlideShow: function() { 
		slideshow = 1; 
		objSlideShowImage.setAttribute('src', SlideShowStopImage); 
		this.slideShowTimer = setTimeout(function() { 
			if (keyPressed) { 
				keyPressed = false; 
				return;
			}
			if(activeImage < (imageArray.length-1)) this.changeImage(activeImage + 1); 
			else { 
				if(foreverLoop) this.changeImage(0); 
				else { 
					slideshow = 0; 
					objSlideShowImage.setAttribute('src', SlideShowStartImage);
				}
			}
		}.bind(this), loopInterval);
	}, 
	stopSlideShow: function() { 
		slideshow = 0; 
		objSlideShowImage.setAttribute('src', SlideShowStartImage); 
		if(this.slideShowTimer) { 
			clearTimeout(this.slideShowTimer); 
			this.slideShowTimer = null; 
			Element.setInnerHTML('flashPlayer', '');
		}
	}, 
	end: function() { 
		this.stopSlideShow(); 
		this.disableKeyboardNav(); 
		Element.hide('lightbox'); 
		new Effect.Fade('overlay', { duration: 0.2 }); 
		showSelectBoxes(); 
		slideshow = saveSlideshow; 
		foreverLoop = saveForeverLoop; 
		loopInterval = saveLoopInterval; 
		slideShowWidth = saveSlideShowWidth; 
		slideShowHeight = saveSlideShowHeight;
	}
}

function getPageScroll() { 
	var yScroll; 
	if (self.pageYOffset) { yScroll = self.pageYOffset; } 
	else if (document.documentElement && document.documentElement.scrollTop) { yScroll = document.documentElement.scrollTop; } 
	else if (document.body) { yScroll = document.body.scrollTop; }
	arrayPageScroll = new Array('',yScroll)
	return arrayPageScroll;
}
function getPageSize() { 
	var xScroll, yScroll; 
	if (window.innerHeight && window.scrollMaxY) { 
		xScroll = document.body.scrollWidth; 
		yScroll = window.innerHeight + window.scrollMaxY;
	} 
	else if (document.body.scrollHeight > document.body.offsetHeight) { 
		xScroll = document.body.scrollWidth; 
		yScroll = document.body.scrollHeight;
	} 
	else { 
		xScroll = document.body.offsetWidth; 
		yScroll = document.body.offsetHeight;
	}
	var windowWidth, windowHeight; 
	if (self.innerHeight) { 
		windowWidth = self.innerWidth; 
		windowHeight = self.innerHeight;
	} 
	else if (document.documentElement && document.documentElement.clientHeight) { 
		windowWidth = document.documentElement.clientWidth; 
		windowHeight = document.documentElement.clientHeight;
	} 
	else if (document.body) { 
		windowWidth = document.body.clientWidth; 
		windowHeight = document.body.clientHeight;
	}
	if(yScroll < windowHeight) { pageHeight = windowHeight; } 
	else { pageHeight = yScroll; }
	if(xScroll < windowWidth) { pageWidth = windowWidth; } 
	else { pageWidth = xScroll; }
	arrayPageSize = new Array(pageWidth,pageHeight,windowWidth,windowHeight)
	return arrayPageSize;
}
function getKey(e) { 
	if (e == null) { keycode = event.keyCode; } 
	else { keycode = e.which; }
	key = String.fromCharCode(keycode).toLowerCase(); 
	if(key == 'x'){ }
}
function listenKey() { 
	document.onkeypress = getKey; 
}
function showSelectBoxes() { 
	selects = document.getElementsByTagName("select"); 
	for (i = 0; i != selects.length; i++) { selects[i].style.visibility = "visible"; }
}
function hideSelectBoxes() { 
	selects = document.getElementsByTagName("select"); 
	for (i = 0; i != selects.length; i++) { selects[i].style.visibility = "hidden"; }
}
function pause(numberMillis) { 
	var now = new Date(); 
	var exitTime = now.getTime() + numberMillis; 
	while (true) { 
		now = new Date(); 
		if (now.getTime() > exitTime) return;
	}
}
function initLightbox() { 
	myLightbox = new Lightbox();
}
function init() { 
	if (arguments.callee.done) return; 
	arguments.callee.done = true; 
	if (_timer) { 
		clearInterval(_timer); 
		_timer = null;
	}
	initLightbox();
}; 
if (document.addEventListener) { 
	document.addEventListener("DOMContentLoaded", init, false);
}
if (/WebKit/i.test(navigator.userAgent)) { 
	var _timer = setInterval(function() { 
		if (/loaded|complete/.test(document.readyState)) { init(); }
	}, 10);
}
window.onload = init; 
