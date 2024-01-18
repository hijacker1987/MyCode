import React from 'react';

const CodesTable = ({ codes, headers }) => {

    return (
        <div className="table-responsive">
            <table className="table table-striped table-hover">
                <thead>
                    <tr>
                        {headers.map(header => (
                            <th key={header} className='table-primary' style={{ color: "darkred" }}>{header}</th>
                        ))}
                    </tr>
                </thead>
                <tbody>
                    {codes && codes.map((code, index) => (
                        <tr key={code.codeTitle} style={{ color: "black", backgroundColor: index % 2 === 1 ? '#f2f2f2' : '' }}>
                            <td>{code.codeTitle}</td>
                            <td>{code.myCode}</td>
                            <td>{code.whatKindOfCode}</td>
                            <td>{code.isBackend ? "Backend" : "Frontend"}</td>
                            <td>{code.isVisible ? "Yes" : "Hidden"}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default CodesTable;
