
const getDomElements = function(){
  console.log("dom geladen");
};

function btnclicked(){
    let mailvalue = document.getElementById('username').value;
    console.log(mailvalue);
    let ww = document.getElementById('password').value;
    console.log(ww);
    let login = mailvalue + '/' + ww
    Server_endpoint = `https://mctproject2.azurewebsites.net/api/v1/login/` + login;

    let customHeaders = new Headers();
    customHeaders.append('Accept', 'application/json');

	return fetch(Server_endpoint, { headers: customHeaders })
		.then(r => r.json())
        .then(data => data);
}

const getApi = async function(){
	const data = await fetchdata(Server_endpoint);
    const feature = data.features;
    console.log(feature);
}




document.addEventListener('DOMContentLoaded', function()
{
    getDomElements();
});