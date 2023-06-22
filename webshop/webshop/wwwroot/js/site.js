// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function handleSelectChange() {
    var selectElement = document.getElementById("mySelect");
    var selectedValue = selectElement.value;

    // Perform actions with the selected value
    // ...
    if (selectedValue === "") {
        Array.from(document.getElementsByClassName("container-product")).forEach((e) => {
            console.log("test");
            e.style.display = "block";

        })
    } else {
        var eenClass = document.getElementsByClassName(selectedValue)

        Array.from(eenClass).forEach((e) => {
            console.log("test");
            e.style.display = "block";
        })

        if (eenClass === selectedValue) {

        } else {
            

            Array.from(document.getElementsByClassName("container-product")).forEach((e) => {
                console.log("test");
                e.style.display = "none";
            })

            Array.from(eenClass).forEach((e) => {
                console.log("test");
                e.style.display = "block";
            })
        }
    }

    console.log("Selected value: " + selectedValue);
}