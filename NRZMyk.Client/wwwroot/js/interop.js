window.saveAsFile = function (fileName, byteBase64) {
    var link = this.document.createElement('a');
    link.download = fileName;
    link.href = "data:application/octet-stream;base64," + byteBase64;
    this.document.body.appendChild(link);
    link.click();
    this.document.body.removeChild(link);
}

window.closeBootstrapModal = function (id)
{
    var modal = window.bootstrap.Modal.getInstance(document.getElementById(id));
    modal.hide();
}

window.selectTabBootstrap = function (number) {
    var someTabTriggerEl = document.querySelector('#nav-home-tab-' + i);
    var tab = new window.bootstrap.Tab(someTabTriggerEl);
    tab.show();
}