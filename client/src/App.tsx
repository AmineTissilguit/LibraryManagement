import { Routes, Route } from "react-router-dom";
import Layout from "./components/layout/Layout";
import Books from "./pages/Books";
import Members from "./pages/Members";

function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<Books />} />
        <Route path="books" element={<Books />} />
        <Route path="members" element={<Members />} />
      </Route>
    </Routes>
  );
}

export default App;
