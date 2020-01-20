let id; 
let decid;
let decid2;
let uri = "https://mctproject2.azurewebsites.net/api/v1/subject?cookie_id=";
let domonderwerpen;
let domvragen;
let quiz_ids = [];
let datavragen = [];
let moeilijkheidsgraad = 0;
let selectedsubject;

const getDomElements = function(){
  console.log("dom geladen");
  id = getCookie('id');
  decid = id.replace("+", "%2B");
};

function postdata(url = '', data= {})
{
  fetch(url, {
    method: 'POST', 
    body: JSON.stringify(data),
  })
  .then((response) => response.json())
  .then((data)=> {
    console.log('success:', data);
  })
  .catch((error)=>{
    console.error('error: ', error);
  })
}

const buttonclick = function(){
  let btn = document.querySelector('.js-btn-onderwerp-verzenden');
  btn.addEventListener('click', function()
  {
    var subjectinput = document.getElementById("js-input-onderwerp");
    let valuesubject = subjectinput.value
    let data = {title: valuesubject, description: ''}

    console.log(subjectinput.value);
    id = getCookie('id');
    decid = encodeURIComponent(id);
    postdata(uri+decid, data)
    document.getElementById("js-input-onderwerp").value = "";  
  })
}

const moeilijkheid1 = function(){
  let makkelijk = document.querySelector('.js-moeilijkheid1')
  makkelijk.addEventListener('click', function(){
    makkelijk.style.backgroundColor="red";
    moeilijkheidsgraad = 0;
    // terug normaal zetten andere variabelen
    let normaal = document.querySelector('.js-moeilijkheid2');
    let moeilijk = document.querySelector('.js-moeilijkheid3');
    normaal.style.backgroundColor="";
    moeilijk.style.backgroundColor="";

  })
}

const moeilijkheid2 = function(){
  let normaal = document.querySelector('.js-moeilijkheid2');
  normaal.addEventListener('click', function(){
    normaal.style.backgroundColor="red";
    moeilijkheidsgraad = 1;
    // terug normaal zetten andere variabelen
    let makkelijk = document.querySelector('.js-moeilijkheid1');
    let moeilijk = document.querySelector('.js-moeilijkheid3');
    makkelijk.style.backgroundColor="";
    moeilijk.style.backgroundColor="";
  })
}

const moeilijkheid3 = function(){
  let moeilijk = document.querySelector('.js-moeilijkheid3');
  moeilijk.addEventListener('click', function(){
    moeilijk.style.backgroundColor="red";
    moeilijkheidsgraad = 2;
    // terug normaal zetten andere variabelen
    let makkelijk = document.querySelector('.js-moeilijkheid1');
    let normaal = document.querySelector('.js-moeilijkheid2');
    makkelijk.style.backgroundColor="";
    normaal.style.backgroundColor="";
  })
}

const buttonclickvragen = function(){
  let btnvragen = document.querySelector('.js-btn-verzenden');
  btnvragen.addEventListener('click', function(){
    let vraag = document.getElementById('js-vraag').value;
    var antwoorda = document.getElementById('js-antwoorda').value;
    var antwoordb = document.getElementById('js-antwoordb').value;
    var antwoordc = document.getElementById('js-antwoordc').value;
    var antwoordd = document.getElementById('js-antwoordd').value;
    var radiobtna = document.getElementById('js-rbtn-a').checked;
    var radiobtnb = document.getElementById('js-rbtn-b').checked;
    var radiobtnc = document.getElementById('js-rbtn-c').checked;
    var radiobtnd = document.getElementById('js-rbtn-d').checked; 
    /* var level = document.getElementById('js-level').value;
    var levelnr = Number(level); */
    var levelnr = Number(moeilijkheidsgraad);

    let data = {
      question: vraag, 
      difficulty: levelnr,
      answers: [
        {
        answer: antwoorda,
        correct: radiobtna
        },
        {
        answer: antwoordb,
        correct: radiobtnb
        },
        {
        answer: antwoordc,
        correct: radiobtnc
        },
        {
        answer: antwoordd,
        correct: radiobtnd
        }
      ]  
    }
    console.log(data);
    /* console.log(data); */
    var subjectinput = document.getElementById("js-input-onderwerp-vragen");
    let valuesubject = subjectinput.value
    for (var quiz_id in quiz_ids)
    {
      if(!quiz_ids.hasOwnProperty(quiz_id)) continue;
      const everything = quiz_ids[quiz_id];
      const qid = everything.quiz_id;
      const title = everything.title;
      if (title == valuesubject)
      {
        console.log(qid);
        localStorage.setItem('quizid', qid);
      }
    }
    const quizid = localStorage.getItem('quizid');
    let onderwerpid = quizid
    let apio = "https://mctproject2.azurewebsites.net/api/v1/question/"+onderwerpid+"?cookie_id=";
   /*  let apionderwerp = "https://mctproject2.azurewebsites.net/api/v1/question/bef11ca2-3fb0-4bdf-90d2-2ad0be4787e6?cookie_id=" */
    id = getCookie('id');
    decid = encodeURIComponent(id);
    
    postdata(apio+decid,data)

  })
}

//get onderwerpen

const getonderwerpen = function(){
  handleData(`https://mctproject2.azurewebsites.net/api/v1/subjects`, showonderwerpen)
}

const showonderwerpen = function(data)
{
  
  domonderwerpen = document.querySelector('.js-selecteditem');
  let arronderwerpen = data
  let OnderwerpHTML = ``;
  
  for (let i = 0; i < data.length; i++)
  {
    let ids = {};
    const quizdata = data[i];
    if(quizdata.quiz_id){
      ids['quiz_id'] = quizdata.quiz_id;
    }
    if (quizdata.title)
    {
      ids['title'] = quizdata.title;
    }
    
    quiz_ids.push(ids);

    OnderwerpHTML += `
    <datalist class="js-selecteditem" id="onderwerpen">
                  <option value=${data[i].title}>
    </datalist>
    `;
   
  }

  domonderwerpen.innerHTML = OnderwerpHTML;
}

//get vragen

const fetchData = async function(url, method = "GET", body = null) {
  return fetch(`https://mctproject2.azurewebsites.net/api/v1/questions/${url}`, {
    method: method,
    body: body,
    headers: { "content-type": "application/json" }
  })
    .then(r => r.json())
    .then(data => data);
};

let getAPI = async function(url, method = "GET", body = null) {
  datavragen = [];
    try {
      const data = await fetchData(url, method, body);
 /*      getrandomnr(0, data.length); */
     /*  console.log(data.error_message) */
      console.log(data);
      for (let i = 0; i < data.length; i++)
      {
          let vragen = {};
          const quizvragen = data[i];
          if (quizvragen)
          {
              vragen['question'] = quizvragen.question;
              vragen['answers'] = quizvragen.answers;
              vragen['difficult'] = quizvragen.difficulty;
          } 
          datavragen.push(vragen);
       
      }

      
    } catch (error) {
      console.log(error);
    }
  };


const buttonclickallevragen = function(){
  let btn = document.querySelector('.js-btn-vragen');
  btn.addEventListener('click', async function()
    {
      selectedsubject=document.querySelector('.js-show_subject');
      console.log(selectedsubject.value);
      let valuesubject = selectedsubject.value
      for (var quiz_id in quiz_ids)
      {
        if(!quiz_ids.hasOwnProperty(quiz_id)) continue;
        const everything = quiz_ids[quiz_id];
        const qid = everything.quiz_id;
        const title = everything.title;
        if (title == valuesubject)
        {
          console.log(qid);
          localStorage.setItem('quizopgevraagdeid', qid);
        }
      }
      const quizid = localStorage.getItem('quizopgevraagdeid');
      await getAPI(quizid);


      domvragen = document.querySelector('.c-form-extra');
      // let arrvragen = datavragen
      let vragenHTML = ``;
      for (let i = 0; i < datavragen.length; i++)
      {
        const quizdata = datavragen[i];
      
        vragenHTML += `<div class="c-form-delete js-form-delete">
        <input class="c-input-vragen" placeholder="${quizdata.question}" id="js-vraag"></input>
        
        <label class="c-lbl-antwoorden c-margin">Antwoorden:</label>
    
        <input class="c-radio-a c-radiobtn-option" ${quizdata.answers[0].correct ? 'checked' : '' } id="js-rbtn-a" type="radio" name="vragen-${i}" value="a"><br>
                
        <input class="c-radio-b c-radiobtn-option" ${quizdata.answers[1].correct ? 'checked' : '' } id="js-rbtn-b" type="radio" name="vragen-${i}" value="b"><br>
            
        <input class="c-radio-c c-radiobtn-option" ${quizdata.answers[2].correct ? 'checked' : '' } id="js-rbtn-c" type="radio" name="vragen-${i}" value="c"><br>
        
        <input class="c-radio-d c-radiobtn-option" ${quizdata.answers[3].correct ? 'checked' : '' } id="js-rbtn-d" type="radio" name="vragen-${i}" value="d">  
        <input class="c-input-a c-input-style" placeholder="${quizdata.answers[0].answer}" id="js-antwoorda"></input>
        <input class="c-input-b c-input-style" placeholder="${quizdata.answers[1].answer}" id="js-antwoordb"></input>
        <input class="c-input-c c-input-style" placeholder="${quizdata.answers[2].answer}" id="js-antwoordc"></input>
        <input class="c-input-d c-input-style" placeholder="${quizdata.answers[3].answer}" id="js-antwoordd"></input>
        <div class="c-div-level c-margin">
            <label class="c-lbl-level ">Moeilijkheid: </label>
            <label class="c-lbl-1 js-moeilijkheid1">${quizdata.difficult}</label>
        </div>
        <div class="c-btn-delete js-btn-verzenden"></div>
    </div> `;
      }
    
      domvragen.innerHTML = vragenHTML;
        })  

}


document.addEventListener('DOMContentLoaded', function()
{
    getDomElements();
    buttonclick();
    buttonclickvragen();
    buttonclickallevragen();
    moeilijkheid1();
    moeilijkheid2();
    moeilijkheid3();
    getonderwerpen();
    /* getAPI("bef11ca2-3fb0-4bdf-90d2-2ad0be4787e6"); */
});