if(document.readyState == 'loading') {
    document.addEventListener('DOMContentLoaded', load, true);
} else {
    load();
}

var checkbox;

function load(){
    document.getElementById("extToggle").addEventListener("click", toggleExt);
    checkbox = document.getElementById("extToggle");
    if(checkbox == null){
        alert("The extention failed to load correctly. Please restart the extention.");
        console.error("checkbox is null!");
        return;
    }
    chrome.storage.local.get(['extToggle'], function(result) {
      checkbox.checked = result.extToggle;
      console.log('Set extention toggle to ' + result.extToggle);
    });
}

function toggleExt(){
    if(checkbox.checked){
        chrome.storage.local.set({'extToggle': true}, function() {
            console.log('Extension is enabled');
          });
    }
    else{
        chrome.storage.local.set({'extToggle': false}, function() {
            console.log('Extension is disabled');
          });
    }
}