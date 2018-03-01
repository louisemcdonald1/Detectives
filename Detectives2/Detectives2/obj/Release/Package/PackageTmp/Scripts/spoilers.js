//called by Details and Delete Views in Books and Films or TV Series
//show spoilers when Show Spoilers button clicked
var event1 = document.getElementById("spoilers");
event1.addEventListener('click', displaySpoilers, false);
function displaySpoilers() {
    document.getElementById("spoilersLocation").style.display = "block";
    //document.getElementById("spoilers").style.display = "none";
}
//hide spoilers when Hide Spoilers button clicked
var event2 = document.getElementById("hideSpoilers");
event2.addEventListener('click', hideSpoilers, false);
function hideSpoilers() {
    document.getElementById("spoilersLocation").style.display = "none";
    document.getElementById("spoilers").style.display = "block";
}