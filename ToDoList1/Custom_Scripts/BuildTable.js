$(document).ready(function () {

    $.ajax({
        url: '/ToDoes/BuildToDoTable', //wywołuje akcje kontrolera 
        success: function (result) { //resultem jest odpowiedz HTML - ten PartialView z akcji kontrolera
            $('#tableDiv').html(result); //jQuery przechwytuje PartialView z akcji kontrolera i wsadza go w miejsce tableDiv
        }
    });
}) 