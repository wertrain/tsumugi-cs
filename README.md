# Tsumugi

Tsumugi is a scripting engine and its development environment.

## Overview

### Tsumugi Text

This is a scripting engine system designed mainly for creating adventure games. It uses "tags" and the scripting language described below to write scripts for adventure games.

### Tsumugi Script

It is a simple scripting language similar to JavaScript.

#### Code

```js
let addX = function(n) { x + n; }
let x = 100;
addX(1);

let hello="world"
hello;
```
#### Output
```
101
world
```
#### Code
```js
let fib = function(n) { if(n == 0) { return 0; } if(n == 1) { return 1;} else { fib(n-1) + fib(n-2) }
fib(20);
```
#### Output
```
6765
```

### Tsumugi Editor

![Tsumugi Editor](https://raw.githubusercontent.com/wertrain/tsumugi-cs/main/Screenshots/editor.png "Tsumugi Editor")

## Credit

Icons used in this software are licensed by http://www.famfamfam.com/lab/icons/silk/  
Application Icons made by <a href="https://www.freepik.com" title="Freepik">Freepik</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a></div>
