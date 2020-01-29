const baseURL = "https://mctproject2.azurewebsites.net/api/v1";
//const baseURL = "http://localhost:7071/api/v1";

const fetchData = async function(url, method = "GET", body = null) {
  //url = encodeURIComponent(url);
  return fetch(`${baseURL}/${url}`, {
    method: method,
    body: body,
    headers: { "content-type": "application/json" }
  })
    .then(r => r.json())
    .then(data => data);
};
let getAPI = async function(url, callback, method = "GET", body = null) {
  try {
    const data = await fetchData(url, method, body);
   /*  console.log(data); */
    callback(data);
  } catch (error) {
   /*  console.log(error); */
  }
};
