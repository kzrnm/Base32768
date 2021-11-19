export function Base32768TextToClipboard() {
    document.getElementById("base32768Text").select();
    document.execCommand("copy");
}