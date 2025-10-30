import React, { useState } from "react";
import FishBoard from "./board/FishBoard";
import AddMarketForm from "../market/AddMarketForm";
import { useAuth } from "../../contexts/authContext";

const Home = () => {
  const { isAuthenticated } = useAuth();
  const [showAddMarket, setShowAddMarket] = useState(false);
  const [refreshKey, setRefreshKey] = useState(0);

  const handleAddMarket = (marketData) => {
    console.log("Market added successfully:", marketData);
    setShowAddMarket(false);
    setRefreshKey((prev) => prev + 1);
  };

  return (
    <div className="min-h-screen py-8">
      <div className="max-w-7xl mx-auto px-4">
        <div className="mb-8">
          <div className="flex justify-between items-center">
            <h1 className="text-4xl font-bold text-gray-900">
              Fish Market Prices
            </h1>
            {isAuthenticated && (
              <button
                className="flex items-center gap-2 bg-teal-600 text-white px-6 py-3 rounded-lg hover:bg-teal-700 transition-colors duration-200 shadow-sm font-medium"
                onClick={() => setShowAddMarket(true)}
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  className="h-5 w-5"
                  viewBox="0 0 20 20"
                  fill="currentColor"
                >
                  <path
                    fillRule="evenodd"
                    d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z"
                    clipRule="evenodd"
                  />
                </svg>
                Add Market
              </button>
            )}
          </div>
        </div>
        <main className="bg-white rounded-lg shadow-md p-8">
          <FishBoard key={refreshKey} />
        </main>
      </div>

      {showAddMarket && (
        <AddMarketForm
          onClose={() => setShowAddMarket(false)}
          onSubmit={handleAddMarket}
        />
      )}
    </div>
  );
};

export default Home;
