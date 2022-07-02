chrome.webRequest.onHeadersReceived.addListener(function(details) {
  // ... your code that checks whether the request should be blocked ...
  //  (omitted for brevity)
  if(details.url.includes(".zip")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  else if(details.url.includes(".7z")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  else if(details.url.includes(".rar")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  else if(details.url.includes(".iso")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  else if(details.url.includes(".exe")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  else if(details.url.includes(".msi")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  else if(details.url.includes(".aif")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  else if(details.url.includes(".cda")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  else if(details.url.includes(".mid")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  else if(details.url.includes(".midi")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  else if(details.url.includes(".mp3")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  else if(details.url.includes(".mpa")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  else if(details.url.includes(".ogg")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  else if(details.url.includes(".wav")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  else if(details.url.includes(".wma")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  else if(details.url.includes(".wpl")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  else if(details.url.includes(".mp4")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  if(details.url.includes(".bin")){
    console.log("Blocking request.");
    return {redirectUrl: "http://robwu.nl/204" };
  }
  else{
    console.log("Request does not contain any download type so has not been blocked.");
  }
}, {
  urls: ["<all_urls>"],
  types: ["main_frame", "sub_frame"]
}, ["responseHeaders", "blocking"]);

chrome.webRequest.onBeforeRequest.addListener(
    function(details)
    {
      console.log (details.url);
      if(details.url.includes(".zip")){
        console.log("Request url contains .zip which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
      else if(details.url.includes(".7z")){
        console.log("Request url contains .7z which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
      else if(details.url.includes(".rar")){
        console.log("Request url contains .rar which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
      else if(details.url.includes(".iso")){
        console.log("Request url contains .iso which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
      else if(details.url.includes(".exe")){
        console.log("Request url contains .exe which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
      else if(details.url.includes(".msi")){
        console.log("Request url contains .msi which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
      else if(details.url.includes(".aif")){
        console.log("Request url contains .aif which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
      else if(details.url.includes(".cda")){
        console.log("Request url contains .cda which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
      else if(details.url.includes(".mid")){
        console.log("Request url contains .mid which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
      else if(details.url.includes(".midi")){
        console.log("Request url contains .midi which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
      else if(details.url.includes(".mp3")){
        console.log("Request url contains .mp3 which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
      else if(details.url.includes(".mpa")){
        console.log("Request url contains .mpa which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
      else if(details.url.includes(".ogg")){
        console.log("Request url contains .ogg which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
      else if(details.url.includes(".wav")){
        console.log("Request url contains .wav which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
      else if(details.url.includes(".wma")){
        console.log("Request url contains .wma which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
      else if(details.url.includes(".wpl")){
        console.log("Request url contains .wpl which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
      else if(details.url.includes(".mp4")){
        console.log("Request url contains .mp4 which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
      if(details.url.includes(".bin")){
        console.log("Request url contains .bin which is a download type. Sending to Download Manager.");
        httpGet('http://localhost:65535/?url="' + details.url + '"');
      }
    },
  
    {urls: ["<all_urls>"]},
  
    ["blocking"]
);

function httpGet(theUrl)
{
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open( "GET", theUrl, false ); // false for synchronous request
    xmlHttp.send( null );
    return xmlHttp.responseText;
}