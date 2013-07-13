$(function () {
    if (typeof (window.FileReader) == "undefined" || typeof (window.FileReader.prototype.readAsDataURL) == "undefined") {
        $("html").addClass('no-fileapi');
    }
    else {
        $('html').addClass('fileapi');
    }
});