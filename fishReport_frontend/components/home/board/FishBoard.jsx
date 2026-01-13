import { useEffect, useState } from "react";
import {
  getAllMarkets,
  deleteMarket,
  addSpeciesToInventory,
  deleteFishFromInventory,
} from "../../../api/markets";
import { getFishForInventory } from "../../../api/fish";
import { getFishImage } from "../../../src/assets/fishImageMap";
import { useAuth } from "../../../contexts/authContext";
import toast from "react-hot-toast";

const FishBoard = () => {
  const { isAuthenticated } = useAuth();
  const [markets, setMarkets] = useState([]);
  const [selectedMarket, setSelectedMarket] = useState(null);
  const [loading, setLoading] = useState(true);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [marketToDelete, setMarketToDelete] = useState(null);
  const [showAddFishMenu, setShowAddFishMenu] = useState(false);
  const [newFish, setNewFish] = useState({ name: "", price: "" });
  const [availableFish, setAvailableFish] = useState([]);

  useEffect(() => {
    const fetchMarkets = async () => {
      try {
        setLoading(true);
        const response = await getAllMarkets();
        console.log("Fetched markets:", response);
        setMarkets(response);
        if (response.length > 0) {
          setSelectedMarket(response[0]);
        }
      } catch {
        // Error handled in API layer
      } finally {
        setLoading(false);
      }
    };
    fetchMarkets();
  }, []);

  const handleDeleteClick = (market) => {
    if (!market.id) return;
    setMarketToDelete(market);
    setShowDeleteModal(true);
  };

  const handleConfirmDelete = async () => {
    if (!marketToDelete?.id) return;

    try {
      await deleteMarket(marketToDelete.id);
      setMarkets((prevMarkets) =>
        prevMarkets.filter((m) => m.id !== marketToDelete.id)
      );
      if (selectedMarket?.id === marketToDelete.id) {
        setSelectedMarket(null);
      }
      setShowDeleteModal(false);
      setMarketToDelete(null);
      toast.success("Market deleted successfully");
    } catch (error) {
      const errorMessage =
        typeof error.response?.data === "string"
          ? error.response.data
          : "Failed to delete market. Please try again.";
      toast.error(errorMessage);
      console.error("Error deleting market:", errorMessage);
      setShowDeleteModal(false);
      setMarketToDelete(null);
    }
  };

  const handleCancelDelete = () => {
    setShowDeleteModal(false);
    setMarketToDelete(null);
  };

  const handleAddFishClick = async () => {
    try {
      const fishList = await getFishForInventory();
      setAvailableFish(fishList);
      setShowAddFishMenu(true);
    } catch {
      // Error handled in API layer
    }
  };

  const handleFishSelect = (fish) => {
    setNewFish(fish);
  };

  const handleAddFishSubmit = async () => {
    if (!selectedMarket?.id || !newFish?.id) return;

    try {
      await addSpeciesToInventory(selectedMarket.id, newFish.id);

      const updatedMarkets = await getAllMarkets();
      setMarkets(updatedMarkets);

      const updatedMarket = updatedMarkets.find(
        (m) => m.id === selectedMarket.id
      );
      if (updatedMarket) {
        setSelectedMarket(updatedMarket);
      }

      setShowAddFishMenu(false);
      setNewFish({ name: "", price: "" });
      toast.success("Fish added to market successfully");
    } catch (error) {
      const errorMessage =
        error.response?.data || "Failed to add fish. Please try again.";
      toast.error(errorMessage);
      console.error("Error adding fish:", error);
    }
  };

  const handleDeleteFish = async (species) => {
    if (!selectedMarket?.id || !species?.id) return;

    try {
      await deleteFishFromInventory(selectedMarket.id, species.id);

      const updatedMarkets = await getAllMarkets();
      setMarkets(updatedMarkets);

      const updatedMarket = updatedMarkets.find(
        (m) => m.id === selectedMarket.id
      );
      if (updatedMarket) {
        setSelectedMarket(updatedMarket);
      }
      toast.success("Fish deleted successfully");
    } catch (error) {
      const errorMessage =
        error.response?.data || "Failed to delete fish. Please try again.";
      toast.error(errorMessage);
      console.error("Error deleting fish:", error);
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center min-h-[400px]">
        <div className="text-xl text-teal-600 animate-pulse">Loading markets...</div>
      </div>
    );
  }

  return (
    <div className="w-full">
      {/* Markets List */}
      <div className="mb-6 flex gap-4">
        <div className="w-1/2 bg-white border border-teal-200 rounded-xl shadow-sm">
          <div className="h-32 overflow-y-auto [&::-webkit-scrollbar]:w-2 [&::-webkit-scrollbar-track]:bg-cyan-50 [&::-webkit-scrollbar-thumb]:bg-teal-400 [&::-webkit-scrollbar-thumb]:rounded-full [&::-webkit-scrollbar-thumb:hover]:bg-teal-500">
            {markets.map((market) => (
              <div
                key={market.marketName}
                className={`flex items-center justify-between border-b border-teal-100 transition-all duration-200 ${
                  selectedMarket?.marketName === market.marketName
                    ? "bg-gradient-to-r from-teal-100 to-cyan-50"
                    : "hover:bg-gradient-to-r hover:from-teal-50 hover:to-cyan-25"
                }`}
              >
                <button
                  onClick={() => setSelectedMarket(market)}
                  className="flex-1 px-4 py-3 text-left text-sm font-medium text-gray-800"
                >
                  <div className="font-semibold text-teal-700">{market.marketName}</div>
                  <div className="text-xs text-gray-500 mt-0.5">{market.location}</div>
                </button>
                {isAuthenticated && (
                  <button
                    onClick={() => handleDeleteClick(market)}
                    className="w-8 h-8 mr-2 rounded-full text-red-600 hover:bg-red-100 hover:text-red-700 transition-all duration-200 flex items-center justify-center text-xl font-light"
                  >
                    ×
                  </button>
                )}
              </div>
            ))}
          </div>
        </div>

        {/* Delete Confirmation Modal */}
        {showDeleteModal && (
          <div className="bg-white p-6 rounded-xl border border-red-200 shadow-lg">
            <div className="mb-4">
              <p className="text-sm font-semibold text-gray-800 mb-1">
                Delete Market?
              </p>
              <p className="text-sm text-gray-600">
                "{marketToDelete?.marketName}" will be permanently removed.
              </p>
            </div>
            <div className="flex justify-end gap-3">
              <button
                onClick={handleCancelDelete}
                className="px-4 py-2 text-sm bg-gray-100 border border-gray-300 rounded-lg hover:bg-gray-200 transition-colors duration-200 font-medium"
              >
                Cancel
              </button>
              <button
                onClick={handleConfirmDelete}
                className="px-4 py-2 text-sm bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors duration-200 font-medium"
              >
                Delete
              </button>
            </div>
          </div>
        )}
      </div>

      {selectedMarket ? (
        <div className="bg-gradient-to-br from-cyan-50 to-teal-50 rounded-xl shadow-sm p-6">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {selectedMarket.species.map((species) => (
              <div
                key={species.name}
                className="bg-white rounded-xl border border-teal-100 shadow-sm hover:shadow-md transition-all duration-300 overflow-hidden group"
              >
                <div className="h-48 bg-gradient-to-br from-teal-100 to-cyan-100 overflow-hidden">
                  <img
                    src={getFishImage(species.name)}
                    alt={species.name}
                    className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
                  />
                </div>
                <div className="p-4">
                  <h4 className="text-lg font-semibold text-gray-800 mb-2">
                    {species.name}
                  </h4>
                  <p className="text-2xl font-bold text-teal-600 mb-4">
                    ${species.price.toFixed(2)}
                  </p>
                  <div className="flex gap-2">
                    <button
                      className="flex-1 bg-red-600 text-white py-2 px-4 rounded-lg hover:bg-red-700 transition-colors duration-200 font-medium"
                      onClick={() => handleDeleteFish(species)}
                    >
                      Delete
                    </button>
                  </div>
                </div>
              </div>
            ))}
            {showAddFishMenu ? (
              <div className="bg-white rounded-xl border border-teal-200 shadow-sm p-5">
                <h4 className="text-sm font-semibold text-teal-700 mb-3">Select Fish</h4>
                <div className="h-32 overflow-y-auto [&::-webkit-scrollbar]:w-2 [&::-webkit-scrollbar-track]:bg-cyan-50 [&::-webkit-scrollbar-thumb]:bg-teal-400 [&::-webkit-scrollbar-thumb]:rounded-full [&::-webkit-scrollbar-thumb:hover]:bg-teal-500">
                  <div className="space-y-2">
                    {availableFish.map((fish) => (
                      <button
                        key={fish.id}
                        onClick={() => handleFishSelect(fish)}
                        className={`w-full px-3 py-2 text-left text-sm rounded-lg flex items-center gap-3 transition-all duration-200 ${
                          newFish.id === fish.id
                            ? "bg-gradient-to-r from-teal-100 to-cyan-100 border border-teal-300"
                            : "hover:bg-teal-50 border border-transparent"
                        }`}
                      >
                        <img
                          src={getFishImage(fish.name)}
                          alt={fish.name}
                          className="w-10 h-10 object-cover rounded-lg"
                        />
                        <div className="flex-1">
                          <div className="font-medium text-gray-800">{fish.name}</div>
                          <div className="text-xs text-teal-600 font-semibold">${fish.price.toFixed(2)}</div>
                        </div>
                      </button>
                    ))}
                  </div>
                </div>
                <div className="mt-4 flex gap-3 justify-end">
                  <button
                    onClick={() => setShowAddFishMenu(false)}
                    className="px-4 py-2 text-sm bg-gray-100 border border-gray-300 rounded-lg hover:bg-gray-200 transition-colors duration-200 font-medium"
                  >
                    Cancel
                  </button>
                  <button
                    onClick={handleAddFishSubmit}
                    disabled={!newFish.id}
                    className="px-4 py-2 text-sm bg-teal-600 text-white rounded-lg hover:bg-teal-700 transition-colors duration-200 font-medium disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    Add
                  </button>
                </div>
              </div>
            ) : isAuthenticated ? (
              <button
                onClick={handleAddFishClick}
                className="bg-white rounded-xl border-2 border-dashed border-teal-300 hover:border-teal-400 shadow-sm hover:shadow-md flex items-center justify-center transition-all duration-300 min-h-[320px] group"
              >
                <div className="text-center">
                  <div className="text-5xl text-teal-400 group-hover:text-teal-500 mb-3 transition-colors duration-200">+</div>
                  <div className="text-base font-semibold text-teal-600 group-hover:text-teal-700 transition-colors duration-200">Add Fish</div>
                </div>
              </button>
            ) : null}
          </div>
        </div>
      ) : (
        <div className="flex justify-center items-center min-h-[400px]">
          <div className="text-xl text-teal-600 font-medium">
            Select a market to view details
          </div>
        </div>
      )}
    </div>
  );
};

export default FishBoard;
