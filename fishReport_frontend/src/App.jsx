import { Routes, Route } from "react-router";
import Home from "../components/home/Home";
import Nav from "../components/Nav";
import FishCollection from "../components/fish/FishCollection";
import AddMarketForm from "../components/market/AddMarketForm";
import { AuthProvider } from "../contexts/authContext";
import { Toaster } from "react-hot-toast";

function App() {
  return (
    <AuthProvider>
      <div>
        <Toaster position="top-left" />
        <Nav />
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/fish" element={<FishCollection />} />
          <Route path="/add-market" element={<AddMarketForm />} />
        </Routes>
      </div>
    </AuthProvider>
  );
}

export default App;
