chrome.webRequest.onBeforeRequest.addListener(
    function(details)
    {
      console.log ("Now playing: " + details.url);
      if(details.url.includes(".png")){
        
      }
    },
  
    {urls: ["<all_urls>"]},
  
    ["blocking"]
  );