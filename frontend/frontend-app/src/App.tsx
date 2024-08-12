//https://www.youtube.com/watch?v=0FRyKY_PMLE

import React, { useState } from 'react';
import './App.css';
import { PDFViewer, Document, Page, Text, Rect, Line } from '@react-pdf/renderer';
import { Svg, G } from './patches/@react-pdf/renderer';

function App() {
  const [fileText, setFileText] = useState<string | null>(null);

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e) => {
        // Decode the result as a UTF-8 string to ensure proper text encoding
        const decodedText = new TextDecoder('utf-8').decode(e.target?.result as ArrayBuffer);
        setFileText(decodedText);
      };
      reader.readAsArrayBuffer(file); // Use readAsArrayBuffer to handle encoding correctly
    }
  };


  return (
    <div className="App">
      <input type="file" onChange={handleFileChange} accept=".txt" />
      <PDFViewer>
        <Document>
          <Page>
            {fileText ? <Text>{fileText}</Text> : <Text>No file selected</Text>}
            <Svg height={200} width={200}>
              <G strokeWidth={4} stroke={'#00F'}>
                <Rect x={25} y={25} height={25} width={50} />
                <Rect x={25} y={25} height={25} width={25} />
              </G>
              <Line x1={100} y1={75} x2={200} y2={25} strokeWidth={4} stroke="#00F" />
            </Svg>
          </Page>
        </Document>
      </PDFViewer>
    </div>
  );
}

export default App;


