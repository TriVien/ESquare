(function () {
    $(function () {
        var basicAuthUI =
            '<div class="input">' +
                '<input placeholder="bearer token" id="input_bearer" name="bearertoken" type="text" size="20">' +
                '<input placeholder="tenant id" id="input_tenant" name="tenantid" type="text" size="10">' +
            '</div>';
        $(basicAuthUI).insertBefore('#api_selector div.input:last-child');
        $("#input_apiKey").hide();
        $('#input_bearer').change(addAuthorization);
        $('#input_tenant').change(addTenantId);

        function addAuthorization() {
            var key = $('#input_bearer')[0].value;
            if (key && key.trim() != "") {
                key = "Bearer " + key;
                swaggerUi.api.clientAuthorizations.add("key", new SwaggerClient.ApiKeyAuthorization("Authorization", key, "header"));
            }
        }

        function addTenantId() {
            var tenant = $('#input_tenant')[0].value;
            if (tenant && tenant.trim() != "") {
                swaggerUi.api.clientAuthorizations.add("tenant", new SwaggerClient.ApiKeyAuthorization("TenantId", tenant, "header"));
            }
        }
    });
})();


