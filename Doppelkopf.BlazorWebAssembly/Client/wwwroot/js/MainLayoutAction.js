var MainPage = MainPage || {};

MainPage.setPageTitle = function (title) {
    document.title = title;
}

MainPage.setMenuTitle = function (title) {
    document.getElementById("menuTitle").innerHTML = title;
}

var EnterTextBox = EnterTextBox || {};

EnterTextBox.clear = function () {
    document.getElementById("enterTextBox").value = "";
}