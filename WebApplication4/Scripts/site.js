let output = null;
const symbols = "+-*/"
let resultInOutput = false
let ans = 0

function loadDoc(){
    output = document.getElementById("output")
    clear_output()
}

function writeToOutput(val) {
    if (!isNaN(val) && resultInOutput === true) clear_output()
    output.value += val
    resultInOutput = false
}

function writeConstant(constant){
    if (symbols.includes(getLastChar()) || getLastChar() === null || resultInOutput === true) writeToOutput(constant);
}

function writeSymbol(symbol){
    if (symbols.includes(getLastChar()) || (getLastChar() === null && symbol !== '-')) return
    writeToOutput(symbol);
}

function getLastChar() {
    if (output.value.length < 1) return null
    return output.value[output.value.length - 1]
}
    
function sendToServer() {
    fetch('https://localhost:44381/home/calculate?val=' + formatOutput())
        .then(response => response.text())
        .then(data => {
            output.value = data
            resultInOutput = true
            ans = data
        })
        .catch(error => console.log(error));
}

function formatOutput(){
    let _output = output.value
    _output = _output.replace("Ans", ans)
    console.log(_output)
    return encodeURIComponent(_output)
}

function delete_char(){
    if (output.value.length < 1) return
    output.value = output.value.slice(0, -1);
}

function clear_output(){
    output.value = ""
}