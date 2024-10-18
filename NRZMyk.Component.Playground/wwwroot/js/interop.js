window.saveAsFile = function (fileName, byteBase64) {
    const link = this.document.createElement('a');
    link.download = fileName;
    link.href = "data:application/octet-stream;base64," + byteBase64;
    this.document.body.appendChild(link);
    link.click();
    this.document.body.removeChild(link);
}

window.closeBootstrapModal = function (id)
{
    const modal = window.bootstrap.Modal.getInstance(document.getElementById(id));
    modal.hide();
}