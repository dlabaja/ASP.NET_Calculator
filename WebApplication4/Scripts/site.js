let output = null;
const symbols = "+-"

function loadDoc(){
    output = document.getElementById("output")
}

function writeToTextBox(val) {
    const symbols = "+-"
    if (output.value === null) {
        output.value += val
        return
    }
    if (symbols.includes(output.value[output.value.length - 1]) && symbols.includes(val)) return //check for duplicate symbols
    output.value += val
}

function writeZero(){
    if (symbols.includes(getLastChar()) || getLastChar() === null) return
    writeToTextBox(0);
}

function writeSymbol(){
    
}

function getLastChar() {
    if (output.value.length < 1) return null
    return output.value[output.value.length - 1]
}
    
function sendToServer() {
    fetch('https://localhost:44381/home/calculate?val=' + encodeURIComponent(output.value))
        .then(response => response.text())
        .then(data => {
            console.log(data)
            output.value = data
        })
        .catch(error => console.log(error));
}

function delete_char(){
    if (output.value.length < 1) return
    output.value = output.value.slice(0, -1);
}

function clear_output(){
    output = ""
}