let input = null;
let output = null;
const symbols = "+-*/"
let ans = 0

function loadDoc(){
    input = document.getElementById("input")
    output = document.getElementById("output")
    clear_output()
}

function writeToOutput(val) {
    if (!isNaN(val) && resultInOutput === true) clear_output()
    input.value += val
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
    if (input.value.length < 1) return null
    return input.value[input.value.length - 1]
}
    
function sendToServer() {
    fetch('https://localhost:44381/home/calculate?val=' + formatOutput())
        .then(response => response.text())
        .then(data => {
            
            input.value = parseFloat(data).toFixed(6)
            resultInOutput = true
            ans = data
        })
        .catch(error => console.log(error));
}

function formatOutput(){
    let _output = input.value.replace("Ans", ans)
    return encodeURIComponent(_output)
}

function delete_char(){
    if (input.value.length < 1) return
    input.value = input.value.slice(0, -1);
}

function clear_output(){
    input.value = ""
}