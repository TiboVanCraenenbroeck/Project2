let domonderwerpen;


const getDomElements = function(){
    console.log("dom geladen");
  };


const getonderwerpen = function(){
    handleData(`https://mctproject2.azurewebsites.net/api/v1/subjects`, showonderwerpen)
 }
  
  const showonderwerpen = function(data)
  {
    console.log(data)
    domonderwerpen = document.querySelectorAll('.js-select');
    /* console.log(data) */
    let OnderwerpHTML = ``;
    for (let i = 0; i< data.length; i++)
    {
      /* console.log(data[0].title);
      console.log(data[i].title) */
      let test = data[i].title
      OnderwerpHTML += `<select name="js-select" id="js-select" class="c-select js-select">
      <option>
          ${test}
      </option>
  </select> `
    }
    domonderwerpen.innerHTML += OnderwerpHTML;
    
  }
 
  document.addEventListener('DOMContentLoaded', function()
{
    getDomElements();
    getonderwerpen();
    domonderwerpen = document.querySelector('.js-select')
});