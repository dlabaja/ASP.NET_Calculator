let input = null;
let output = null;
const symbols = "+-*/%"
const constants = ["π", "e", "Ans"]
let ans = 0

function loadDoc() {
    input = document.getElementById("input")
    output = document.getElementById("output")
    output.value = ""
    clear_input()
    loadLastResults()
}

function writeToInput(val) {
    input.value += val
}

function writeFuncConst(constant) {
    if (symbols.includes(getLastChar()) || getLastChar() === null || getLastChar() === '(') writeToInput(constant);
}

function writeDecimalPoint() {
    if (!isNaN(getLastChar())) writeToInput('.');
}

function writeSymbol(symbol) {
    if (!symbols.includes(getLastChar())) writeToInput(symbol);
}

function getLastChar() {
    if (input.value.length < 1) return null
    return input.value[input.value.length - 1]
}

function sendToServer() {
    fetch(`${window.location.href}/calculate?val=${formatInput()}`)
        .then(response => response.text())
        .then(data => {
            output.value = data
            ans = data

            loadLastResults()
        })
        .catch(error => console.log(error));
}

function loadLastResults() {
    fetch(`${window.location.href}/lastresults`)
        .then(response => response.text())
        .then(json => {
            let data = JSON.parse(json)

            let doc = document.getElementById('table');
            doc.innerHTML = ""
            for (var i = 0; i < data.Results.length; i++) {
                doc.innerHTML += `<tr>
                    <td>${data.Results[i].expression}</td>
                    <td>${data.Results[i].result}</td>
                    <td>${data.Results[i].dateTime}</td>
                </tr>`;
            }
        })
        .catch(error => console.log(error));
}

function formatInput() {
    let _input = input.value.replaceAll("Ans", ans)
    return encodeURIComponent(_input)
}

function delete_char() {
    if (input.value.length < 1) return
    input.value = input.value.slice(0, -1);
}

function clear_input() {
    input.value = ""
}
