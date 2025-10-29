import { useEffect, useState } from "react";
import { getAllFish, updateFishPrice } from "../../api/fish";
import { getFishImage } from "../../src/assets/fishImageMap";

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
      setFish(
        fish.map((f) =>
          f.id === fishId ? { ...f, price: parseFloat(newPrice) } : f
        )
      );
      setEditingFishId(null);
      setNewPrice("");
    } catch (error) {
      console.error("Error updating price:", error);
      alert("Failed to update price. Please try again.");
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
      <div className="mb-6">
        <h2 className="text-2xl font-bold text-gray-800">Fish Collection</h2>
        <p className="text-gray-600 mt-2">
          Browse our collection of fish species
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {fish.map((fish) => (
          <div
            key={fish.id}
            className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow duration-200"
          >
            <div className="h-48 bg-gray-200 flex items-center justify-center overflow-hidden">
              <img
                src={getFishImage(fish.name)}
                alt={fish.name}
                className="w-full h-full object-cover"
              />
            </div>

            <div className="p-6">
              <h3 className="text-xl font-semibold text-gray-800 mb-2">
                {fish.name}
              </h3>

              <div className="space-y-2 text-gray-600">
                <div className="flex justify-between">
                  <span className="font-medium">Habitat:</span>
                  <span>{fish.habitat}</span>
                </div>
                <div className="flex justify-between">
                  <span className="font-medium">Length:</span>
                  <span>{fish.length} cm</span>
                </div>
                <div className="flex justify-between">
                  <span className="font-medium">Population:</span>
                  <span>{fish.population.toLocaleString()}</span>
                </div>
                <div className="flex justify-between">
                  <span className="font-medium">Lifespan:</span>
                  <span>{fish.lifespan} years</span>
                </div>
                <div className="flex justify-between items-center">
                  <span className="font-medium">Price:</span>
                  {editingFishId === fish.id ? (
                    <div className="flex items-center gap-2">
                      <input
                        type="number"
                        value={newPrice}
                        onChange={(e) => setNewPrice(e.target.value)}
                        className="w-24 px-2 py-1 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                        min="0"
                        step="0.01"
                      />
                      <button
                        onClick={() => handleSavePrice(fish.id)}
                        className="text-sm bg-green-600 text-white px-3 py-1 rounded-md hover:bg-green-700"
                      >
                        Save
                      </button>
                      <button
                        onClick={handleCancelEdit}
                        className="text-sm bg-gray-600 text-white px-3 py-1 rounded-md hover:bg-gray-700"
                      >
                        Cancel
                      </button>
                    </div>
                  ) : (
                    <span className="text-blue-600 font-bold">
                      ${fish.price.toFixed(2)}
                    </span>
                  )}
                </div>
              </div>

              <div className="mt-6 flex gap-3">
                <button
                  className="flex-1 bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 transition-colors duration-200 font-medium"
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
