/// <reference path="jquery-1.8.2-vsdoc.js">
$(function () {    
    // create the signalR hub
    var picHub = $.connection.picHub;
    picHub.addImage = function (url) {            
        $("#noPics").css('display', 'none');
        $("#pics :nth-child(6)").remove();
        $("#pics").prepend("<img src='" + url + "'>");
    }
    $.connection.hub.start();

    $("body").bind("drop", function (evt) {
        evt.stopPropagation();
        evt.preventDefault();

        var filelist = event.dataTransfer.files;
        if (!filelist) return;  // if null, exit now
        var filecount = filelist.length;  // get number of dropped files

        if (filecount > 0) {
            // Do something with the files.         
        }

        var files = evt.originalEvent.dataTransfer.files;
        if (files.length > 0) {
            var data = new FormData();
            for (i = 0; i < files.length; i++) {
                data.append("items[]", files[i]);
            }
            console.log('uploading file to server....');
            $.ajax({
                type: "POST",
                url: window.location.href,
                contentType: false,
                processData: false,
                data: data,
                success: function (res) {
                    console.log(res);                                        
                    picHub.update(res);
                }
            });
        }
    }).bind("dragover", function (evt) {
        evt.stopPropagation();
        evt.preventDefault();
    });
})