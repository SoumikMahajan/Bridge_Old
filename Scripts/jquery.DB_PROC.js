
function DB_SP_CONNECT () {
    this.obj = {};
    this.ProcedureName = "";
    this.encodemode = "OFF";
    this.isasync = true;
    this.call_DB_Procedure_Multi = function () {

        var obj = this.obj;
        var ProcedureName = this.ProcedureName;
        var encodemode = this.encodemode;
        var isasync = this.isasync;

        var obj = JSON.stringify(obj);


        var data = "";

        var tkn = $('#hdn_tokn_session_valid').val();

        data = "{\"data\": \"" + escape(obj) + "\", \"ProcedureName\":\"" + ProcedureName + "\",\"encodemode\":\"" + encodemode + "\",\"tkn\" : \"" + tkn + "\"}";

        var datret = "";

        var url = '';

        url = '/Home/SP_Call_Multi';
        //url = '/Home/SP_Call_Multi';
       

        $.ajax({
            type: "POST",
            url: url,
            headers: { 'x-auth-header': tkn },
            contentType: "application/json; charset=utf-8",
            async: isasync,
            data: data,
            success: function (data) {
                datret = JSON.parse(data);
            },
            failed: function () {

            }

        });

        return datret;

    }

    
    this.call_DB_Procedure = function() {
            
            var obj = this.obj;
            var ProcedureName = this.ProcedureName;
            var encodemode = this.encodemode;
            var isasync = this.isasync;

            var obj = JSON.stringify(obj);

          
            var data = "";
      
            var tkn = $('#hdn_tokn_session_valid').val();
           
            data = "{\"data\": \"" + escape(obj) + "\", \"ProcedureName\":\"" + ProcedureName + "\",\"encodemode\":\"" + encodemode + "\",\"tkn\" : \"" + tkn + "\"}";


            var datret = "";
        
            var url = '';
       
            url = '/Home/SP_Call';
            //url = '/Home/SP_Call';
      

            $.ajax({
                type: "POST",
                url: url,
                contentType: "application/json; charset=utf-8",
                async: isasync,
                data: data,
                success: function (data) {
                    datret = JSON.parse(data);
                },
                failed: function () {

                }

            });

            return datret;

        }


   
} 
