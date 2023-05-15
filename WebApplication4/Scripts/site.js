﻿let input = null;
let output = null;
const symbols = "+-*/%"
const constants = ["π", "e", "Ans"]
let ans = 0

function loadDoc() {
    input = document.getElementById("input")
    output = document.getElementById("output")
    output.value = ""
    clear_input()
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
    fetch('https://localhost:44381/home/calculate?val=' + formatInput())
        .then(response => response.text())
        .then(data => {
            output.value = data
            ans = data
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
