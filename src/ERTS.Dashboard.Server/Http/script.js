var canvas = document.querySelector('#canvas');
var output = document.querySelector('#output pre');
var status_el = document.querySelector('#status pre');

var pitch = 0;  // In degree in the range [-90,90]
var yaw = 0; // In degree in the range [-180,180]
var roll = 0; // In degree in the range [-180,180]
var lift = 0; // [0,1]

var width = 40;
var height = 40;

var maxX = canvas.clientWidth;
var maxY = canvas.clientHeight;

var post_enabled = true;
var status_string = 'Initializing';

$('#lift').rangeslider({
  polyfill: false,
  onSlide: function(position, value) { lift = value; }
});

// Disable scrolling.
document.ontouchmove = function (e) {
  e.preventDefault();
}

var ctx = canvas.getContext('2d');

function handleOrientation(event) {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  pitch = event.beta || 0;
  yaw = event.alpha || 0;
  roll = event.gamma || 0;

  if (pitch >  90) { pitch =  90};
  if (pitch < -90) { pitch = -90};

  if (yaw > 180) { yaw -= 360}

  var x = maxX*(roll + 90)/180;
  var y = maxY*(pitch + 90)/180;

  ctx.save();
  ctx.translate(x, y);
  ctx.rotate(-yaw * Math.PI / 180);
  ctx.beginPath();
  ctx.moveTo(0, -height / 2);
  ctx.lineTo(-width / 2, height / 2);
  ctx.lineTo(width / 2, height / 2);
  ctx.closePath();
  ctx.fill();
  ctx.restore();
}

function sendData(data) {
  var XHR = new XMLHttpRequest();
  var urlEncodedData = "";
  var urlEncodedDataPairs = [];
  var name;

  // Turn the data object into an array of URL-encoded key/value pairs.
  for(name in data) {
    urlEncodedDataPairs.push(encodeURIComponent(name) + '=' + encodeURIComponent(data[name]));
  }

  // Combine the pairs into a single string and replace all %-encoded spaces to
  // the '+' character; matches the behaviour of browser form submissions.
  urlEncodedData = urlEncodedDataPairs.join('&').replace(/%20/g, '+');

  // Define what happens on successful data submission
  XHR.addEventListener('load', function(event) {
    if (XHR.status != 200) {
      console.log('Got status code ' + XHR.status + ', disabling requests.');
      post_enabled = false;
      status_string = 'Error: Got invalid response.'
      status_el.style.color = 'red';
    }
    else {
      status_string = 'Connected.'
      status_el.style.color = 'green';
    }
  });

  // Define what happens in case of error
  XHR.addEventListener('error', function(event) {
    console.log('Got error, disabling requests.');
    post_enabled = false;
    status_string = 'Error: Got error.'
    status_el.style.color = 'red';
  });

  // Set up our request
  XHR.open('POST', window.location.origin + '/input');

  // Add the required HTTP header for form data POST requests
  XHR.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');

  // Finally, send our data.
  XHR.send(urlEncodedData);
}

function tick() {
  output.innerHTML  = "pitch:&emsp;" + pitch.toFixed(3) + "\n";
  output.innerHTML += "yaw:&emsp;&emsp;&emsp;" + yaw.toFixed(3) + "\n";
  output.innerHTML += "roll:&emsp;&emsp;" + roll.toFixed(3) + "\n";
  output.innerHTML += "lift:&emsp;&emsp;" + lift.toFixed(3) + "\n";

  status_el.innerHTML = status_string;

  if (post_enabled)
    sendData({
      pitch: pitch / 90,
      yaw: yaw / 180,
      roll: roll / 180,
      lift: lift
    });
};

window.addEventListener('deviceorientation', handleOrientation);
setInterval(tick, 25);
