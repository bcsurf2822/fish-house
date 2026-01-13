import { useEffect, useState } from "react";
import { getAllFish, updateFishPrice } from "../../api/fish";
import { getFishImage } from "../../src/assets/fishImageMap";
import toast from "react-hot-toast";

const FishCollection = () => {
  const [fish, setFish] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [editingFishId, setEditingFishId] = useState(null);
  const [newPrice, setNewPrice] = useState("");

  useEffect(() => {
    const fetchFish = async () => {
      try {
        setLoading(true);
        setError(null);
        const response = await getAllFish();
        setFish(response);
      } catch (error) {
        console.error("Error fetching fish:", error);
        setError("Failed to load fish data. Please try again later.");
      } finally {
        setLoading(false);
      }
    };

    fetchFish();
  }, []);

  function handleEditPrice(event) {
    const { fishId, price } = event.currentTarget.dataset;
    setEditingFishId(parseInt(fishId));
    setNewPrice(price);
  }

  const handleSavePrice = async (fishId) => {
    try {
      await updateFishPrice(fishId, parseFloat(newPrice));

      // Re-fetch data from server to ensure consistency
      const response = await getAllFish();
      setFish(response);

      setEditingFishId(null);
      setNewPrice("");
      toast.success("Price updated successfully");
    } catch (error) {
      console.error("Error updating price:", error);
      const errorMessage =
        error.response?.data || "Failed to update price. Please try again.";
      toast.error(errorMessage);
    }
  };

  const handleCancelEdit = () => {
    setEditingFishId(null);
    setNewPrice("");
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center min-h-[400px]">
        <div className="text-xl text-gray-600">Loading fish data...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex justify-center items-center min-h-[400px]">
        <div className="text-xl text-red-600">{error}</div>
      </div>
    );
  }

  return (
    <div className="w-full">
      <div className="mb-8">
        <h2 className="text-3xl font-bold text-gray-800">Fish Collection</h2>
        <p className="text-gray-600 mt-2">
          Browse our collection of fish species
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {fish.map((fish) => (
          <div
            key={fish.id}
            className="bg-white rounded-xl border shadow-sm overflow-hidden hover:shadow-lg hover:scale-[1.02] transition-all duration-300"
          >
            <div className="h-48 bg-gradient-to-br from-cyan-50 to-teal-50 flex items-center justify-center overflow-hidden">
              <img
                src={getFishImage(fish.name)}
                alt={fish.name}
                className="w-full h-48 object-cover rounded-t-xl"
              />
            </div>

            <div className="p-6">
              <h3 className="text-2xl font-bold text-gray-800 mb-4">
                {fish.name}
              </h3>

              <div className="space-y-2.5 mb-4">
                <div className="flex justify-between text-sm">
                  <span className="text-gray-500">Habitat:</span>
                  <span className="text-gray-600 font-medium">{fish.habitat}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-gray-500">Length:</span>
                  <span className="text-gray-600 font-medium">{fish.length} cm</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-gray-500">Population:</span>
                  <span className="text-gray-600 font-medium">{fish.population.toLocaleString()}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-gray-500">Lifespan:</span>
                  <span className="text-gray-600 font-medium">{fish.lifespan} years</span>
                </div>
              </div>

              <div className="border-t pt-4 mb-4">
                <div className="flex justify-between items-center">
                  <span className="text-sm text-gray-500">Price:</span>
                  {editingFishId === fish.id ? (
                    <div className="flex items-center gap-2">
                      <input
                        type="number"
                        value={newPrice}
                        onChange={(e) => setNewPrice(e.target.value)}
                        className="w-24 px-2 py-1 border border-teal-300 rounded-lg focus:ring-teal-500 focus:border-teal-500"
                        min="0"
                        step="0.01"
                      />
                      <button
                        onClick={() => handleSavePrice(fish.id)}
                        className="text-sm bg-teal-600 text-white px-3 py-1 rounded-lg hover:bg-teal-700 transition-colors"
                      >
                        Save
                      </button>
                      <button
                        onClick={handleCancelEdit}
                        className="text-sm bg-gray-500 text-white px-3 py-1 rounded-lg hover:bg-gray-600 transition-colors"
                      >
                        Cancel
                      </button>
                    </div>
                  ) : (
                    <span className="text-2xl font-bold text-teal-600">
                      ${fish.price.toFixed(2)}
                    </span>
                  )}
                </div>
              </div>

              <div className="flex gap-3">
                <button
                  className="flex-1 bg-teal-600 text-white py-2.5 px-4 rounded-lg hover:bg-teal-700 transition-all duration-200 font-medium shadow-sm hover:shadow-md"
                  onClick={handleEditPrice}
                  data-fish-id={fish.id}
                  data-price={fish.price}
                >
                  Edit Price
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default FishCollection;
