import React, { useState, useEffect } from "react";
import './ui/PDFViewer.css';

import { Viewer, Worker } from '@react-pdf-viewer/core';
import { defaultLayoutPlugin, DefaultLayoutPlugin } from "@react-pdf-viewer/default-layout";
import '@react-pdf-viewer/core/lib/styles/index.css';
import '@react-pdf-viewer/default-layout/lib/styles/index.css';

function PDFViewer(): JSX.Element {
    const [viewPdf, setViewPdf] = useState<string | null>(null);

    // Initialize PDF viewer plugin
    const newPlugin: DefaultLayoutPlugin = defaultLayoutPlugin();

    // Load the PDF file from the public folder when the component mounts
    useEffect(() => {
        // Set the view PDF to a file from the public folder
        setViewPdf('/pdfs/zap2049_olimpiadi_01092023.pdf');  // Adjust the file name as needed
    }, []);


    return (
        <div className="container">
            <h2>View PDF</h2>
            <div className="pdf-container">
                <Worker workerUrl="https://unpkg.com/pdfjs-dist@3.4.120/build/pdf.worker.min.js">
                    {viewPdf ? (
                        <Viewer fileUrl={viewPdf} plugins={[newPlugin]} />
                    ) : (
                        <p>No PDF</p>
                    )}
                </Worker>
            </div>
        </div>
    );
}

export default PDFViewer;