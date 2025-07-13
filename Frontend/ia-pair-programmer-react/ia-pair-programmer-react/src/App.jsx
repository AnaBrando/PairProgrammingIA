import { useState } from 'react'

function App() {
  const [codigo, setCodigo] = useState('')
  const [resposta, setResposta] = useState(null)

  const enviar = async () => {
    const res = await fetch('http://localhost:5000/analise', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ codigo })
    })

    const data = await res.json()
    setResposta(data)
  }

  return (
    <div className="max-w-4xl mx-auto mt-10 p-4 bg-white shadow rounded">
      <h1 className="text-2xl font-bold mb-4">IA Pair Programmer for .NET</h1>

      <textarea
        value={codigo}
        onChange={(e) => setCodigo(e.target.value)}
        placeholder="Paste your C# code here..."
        rows="10"
        className="w-full border p-3 font-mono text-sm rounded mb-4"
      />

      <button
        onClick={enviar}
        className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
      >
        Analyze
      </button>

      {resposta && (
        <div className="mt-6 space-y-4">
          <div>
            <h2 className="font-semibold">Improvement</h2>
            <pre className="bg-gray-100 p-2 rounded">{resposta.melhoria}</pre>
          </div>
          <div>
            <h2 className="font-semibold">Documentation</h2>
            <pre className="bg-gray-100 p-2 rounded">{resposta.documentacao}</pre>
          </div>
          <div>
            <h2 className="font-semibold">Explanation</h2>
            <pre className="bg-gray-100 p-2 rounded">{resposta.explicacao}</pre>
          </div>
          <div>
            <h2 className="font-semibold">Unit Test</h2>
            <pre className="bg-gray-100 p-2 rounded">{resposta.testeUnitario}</pre>
          </div>
        </div>
      )}
    </div>
  )
}

export default App