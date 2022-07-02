var zip;
var sz;
var rar;
var iso;
var exe;
var msi;
var aif;
var cda;
var mid;
var midi;
var mp3;
var mpa;
var ogg;
var wav;
var wma;
var wpl;
var mp4;
var bin;
var saveButton;

window.addEventListener("load", Load);

function Load(){
    saveButton = document.getElementById("save");
    if(saveButton == null){
        console.error("SaveButton is null");
    }
    else{
        console.log("SaveButton is not null");
        saveButton.addEventListener("click", SaveSettings);
    }
    zip = document.getElementById("zip");
    sz = document.getElementById("7z");
    rar = document.getElementById("rar");
    iso = document.getElementById("iso");
    exe = document.getElementById("exe");
    msi = document.getElementById("msi");
    aif = document.getElementById("aif");
    cda = document.getElementById("cda");
    mid = document.getElementById("mid");
    midi = document.getElementById("midi");
    mp3 = document.getElementById("mp3");
    mpa = document.getElementById("mpa");
    ogg = document.getElementById("ogg");
    wav = document.getElementById("wav");
    wma = document.getElementById("wma");
    wpl = document.getElementById("wpl");
    mp4 = document.getElementById("mp4");
    bin = document.getElementById("bin");

    if(getCookie("zip") == "false"){
        zip.removeAttribute("checked");
    }
    if(getCookie("7z") == "false"){
        sz.removeAttribute("checked");
    }
    if(getCookie("rar") == "false"){
        rar.removeAttribute("checked");
    }
    if(getCookie("iso") == "false"){
        iso.removeAttribute("checked");
    }
    if(getCookie("exe") == "false"){
        exe.removeAttribute("checked");
    }
    if(getCookie("msi") == "false"){
        msi.removeAttribute("checked");
    }
    if(getCookie("aif") == "false"){
        aif.removeAttribute("checked");
    }
    if(getCookie("cda") == "false"){
        cda.removeAttribute("checked");
    }
    if(getCookie("mid") == "false"){
        mid.removeAttribute("checked");
    }
    if(getCookie("midi") == "false"){
        midi.removeAttribute("checked");
    }
    if(getCookie("mp3") == "false"){
        mp3.removeAttribute("checked");
    }
    if(getCookie("mpa") == "false"){
        mpa.removeAttribute("checked");
    }
    if(getCookie("ogg") == "false"){
        ogg.removeAttribute("checked");
    }
    if(getCookie("wav") == "false"){
        wav.removeAttribute("checked");
    }
    if(getCookie("wma") == "false"){
        wma.removeAttribute("checked");
    }
    if(getCookie("wpl") == "false"){
        wpl.removeAttribute("checked");
    }
    if(getCookie("mp4") == "false"){
        mp4.removeAttribute("checked");
    }
    if(getCookie("bin") == "false"){
        bin.removeAttribute("checked");
    }
}

function SaveSettings(){
    if(zip.checked){
        document.cookie = "zip=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";    
    }
    else{
        document.cookie = "zip=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    if(sz.checked){
        document.cookie = "7z=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";    
    }
    else{
        document.cookie = "7z=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    if(rar.checked){
        document.cookie = "rar=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";    
    }
    else{
        document.cookie = "rar=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    if(iso.checked){
        document.cookie = "iso=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";    
    }
    else{
        document.cookie = "iso=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    if(exe.checked){
        document.cookie = "exe=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";    
    }
    else{
        document.cookie = "exe=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    if(msi.checked){
        document.cookie = "msi=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";    
    }
    else{
        document.cookie = "msi=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    if(aif.checked){
        document.cookie = "aif=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";    
    }
    else{
        document.cookie = "aif=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    if(cda.checked){
        document.cookie = "cda=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";    
    }
    else{
        document.cookie = "cda=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    if(mid.checked){
        document.cookie = "mid=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";    
    }
    else{
        document.cookie = "mid=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    if(midi.checked){
        document.cookie = "midi=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";    
    }
    else{
        document.cookie = "midi=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    if(mp3.checked){
        document.cookie = "mp3=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";    
    }
    else{
        document.cookie = "mp3=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    if(mpa.checked){
        document.cookie = "mpa=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";    
    }
    else{
        document.cookie = "mpa=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    if(ogg.checked){
        document.cookie = "ogg=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    else{
        document.cookie = "ogg=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    if(wav.checked){
        document.cookie = "wav=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    else{
        document.cookie = "wav=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    if(wma.checked){
        document.cookie = "wma=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    else{
        document.cookie = "wma=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    if(wpl.checked){
        document.cookie = "wpl=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";    
    }
    else{
        document.cookie = "wpl=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    if(mp4.checked){
        document.cookie = "mp4=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";    
    }
    else{
        document.cookie = "mp4=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    if(bin.checked){
        document.cookie = "bin=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";    
    }
    else{
        document.cookie = "bin=false; expires=Tue, 19 Jan 2038 04:14:07 GMT";
    }
    alert("Settings saved!");
    //document.cookie = "cookieName=true; expires=Tue, 19 Jan 2038 04:14:07 GMT";
}

function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for(let i = 0; i <ca.length; i++) {
      let c = ca[i];
      while (c.charAt(0) == ' ') {
        c = c.substring(1);
      }
      if (c.indexOf(name) == 0) {
        return c.substring(name.length, c.length);
      }
    }
    return "";
  }