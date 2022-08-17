document.addEventListener('DOMContentLoaded', load, false);
var serverPortText;
var serverPort;

function load(){
    document.getElementById("submit").addEventListener("click", submitForm);
    serverPortText = document.getElementById("serverPort");
    if(serverPortText == null){
        alert("The extention failed to load correctly. Please restart the extention.");
        console.error("serverPortText is null!");
        return;
    }

    serverPort = chrome.storage.local.get(['port'], function(result) {
       if(result.port == null || result.port == ""){
           chrome.storage.local.set({"port": "65535"}, function() {
               serverPortText.value = result.port;
               serverPort = result.port;
               console.log('Set server port to ' + result.port);
           });
       }
       serverPortText.value = result.port;
       serverPort = result.port;
       console.log('Port currently is ' + result.port);
    });
}

function submitForm(){

    if(serverPort == null || serverPortText == null){
        console.error("serverPort or serverPortText is null!");
    }
    else{
        serverPort = serverPortText.value;

        chrome.storage.local.set({"port": serverPort}, function() {
            console.log('Set server port to: ' + serverPort);
          });
    }
}

/*chrome.storage.local.set({key: value}, function() {
    console.log('Value is set to ' + value);
  });
  
  chrome.storage.local.get(['key'], function(result) {
    console.log('Value currently is ' + result.key);
  });*/