var element = document.getElementById('trick');
new ResizeSensor(element, function () {
    console.log('Changed to ' + element.clientWidth);
    trick_resize();
});



function trick_resize() {
    console.log("hi2")
    var trick = document.getElementById("trick");
    var trickSize = trick.getBoundingClientRect();

    var min = Math.min(trickSize.width, trickSize.height);

    var child = trick.children[0];

    child.style.width = min + "px";
    child.style.height = min + "px";

    console.log("hi2")
}