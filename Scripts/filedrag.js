

(function () {    

	// getElementById
	function $id(id) {
		return document.getElementById(id);
	}


	// output information
	function Output(msg) {
		var m = $id("messages");
		m.innerHTML = msg + m.innerHTML;
	}

	// file drag hover
	function FileDragHover(e) {
		e.stopPropagation();
		e.preventDefault();
		e.target.className = (e.type == "dragover" ? "hover" : "");
	}


	// file selection
	function FileSelectHandler(e) {

		// cancel event and hover styling
		FileDragHover(e);

		// fetch FileList object
		var files = e.target.files || e.dataTransfer.files;

		// process all File objects
		for (var i = 0, f; f = files[i]; i++) {
			ParseFile(f);
		}

	}


	// output file information
	function ParseFile(file) {

	    var fileName = file.name;  
	    var ind = fileName.lastIndexOf(".");
	    var getExtension = fileName.substring(ind+1);

	    if (getExtension == "doc" || getExtension == "docx" || getExtension == "xls" || getExtension == "xlsx") {	      

	        Output(
                "<p name='" + file.name + "' class='getFile' style='font-weight:bold'>File Name : " + file.name + " <img src='../Images/delete-icon.png' height='18px' onclick='DelImage(this)' /></P>"
            );

	        uploadclicked();
	    }
	    else {
	        alert('Invalid File.Supportable format doc or docx or xls or xlsx');
	    }
	}


	// initialize
	function Init() {

		var fileselect = $id("fileselect"),
			filedrag = $id("filedrag"),
			submitbutton = $id("submitbutton");

		// file select
		fileselect.addEventListener("change", FileSelectHandler, false);

		// is XHR2 available?
		var xhr = new XMLHttpRequest();
		if (xhr.upload) {

			// file drop
			filedrag.addEventListener("dragover", FileDragHover, false);
			filedrag.addEventListener("dragleave", FileDragHover, false);
			filedrag.addEventListener("drop", FileSelectHandler, false);
			filedrag.style.display = "block";

			// remove submit button
			submitbutton.style.display = "none";
		}

	}

	// call initialization file
	if (window.File && window.FileList && window.FileReader) {
		Init();
	}


})();


function uploadclicked() {
    var formData = new FormData();
    var totalFiles = document.getElementById("fileselect").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("fileselect").files[i];

        formData.append("fileselect", file);
    }
    var url1 = '';
    if (LOCALORSERVERNEW == 'LOCAL') {
        url1 = 'http://www.thebridgebytcg.com/UploadFile/Upload';
    }
    else if (LOCALORSERVERNEW == 'SERVER') {
        url1 = 'UploadFile/Upload';
    }
    $.ajax({
        type: "POST",
        url: url1,
        data: formData,
        //async:false,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            alert('success!!');
        },
        error: function (error) {
            alert("errror");
        }
    });
}