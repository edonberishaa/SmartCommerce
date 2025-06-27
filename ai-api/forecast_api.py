from fastapi import FastAPI
from pydantic import BaseModel
from typing import List
import pandas as pd
from prophet import Prophet

app = FastAPI()

class SalesData(BaseModel):
    date: str
    quantity: int

class ForecastRequest(BaseModel):
    sales_data: List[SalesData]

@app.post("/forecast")
def forecast(request: ForecastRequest):
    df = pd.DataFrame([{"ds": d.date, "y": d.quantity} for d in request.sales_data])
    
    model = Prophet()
    model.fit(df)
    
    future = model.make_future_dataframe(periods=30)
    forecast = model.predict(future)
    
    result = forecast[["ds", "yhat"]].tail(30).to_dict(orient="records")
    return {"forecast": result}
