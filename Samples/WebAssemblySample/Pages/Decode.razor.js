
function b64toBlob(base64, type = 'application/octet-stream') {
    return fetch(`data:${type};base64,${base64}`).then(res => res.blob())
}

async function BlazorDownloadFile(filename, contentType, content) {
    const data = await b64toBlob(content, contentType);

    const file = new File([data], filename, { type: contentType });
    const exportUrl = URL.createObjectURL(file);

    const a = document.createElement("a");
    document.body.appendChild(a);
    a.href = exportUrl;
    a.download = filename;
    a.target = "_self";
    a.click();

    URL.revokeObjectURL(exportUrl);
}