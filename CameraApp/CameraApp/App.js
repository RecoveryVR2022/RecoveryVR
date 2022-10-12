import React, { useRef, useEffect} from 'react';
import RVR_logo from './RVR_logo.png';

function App() {
  const videoRef = useRef(null);

  const getVideo = () => {
    navigator.mediaDevices
      .getUserMedia({
        video: {width:1920, height: 1080}
      })
      .then(stream =>{
        let video = videoRef.current;
        video.srcObject = stream;
        video.play();
      })
      .catch(err => {
        console.error(err);
      })
  }

  useEffect(() => {
    getVideo();
  }, [videoRef]);

  return (
    <div className="App">
      <div>
        <img src={RVR_logo} alt="Logo" />
      </div>
      <div className="camera">
        <video ref={videoRef}></video>
      </div>
    </div>
  );
}

export default App;




