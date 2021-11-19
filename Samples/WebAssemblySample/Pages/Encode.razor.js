export async function CopyTextToClipboard(text) {
    await navigator.clipboard.writeText(text)
}